using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.RateLimit;
using Polly.Retry;
using HackerAPI.Model;
using HackerAPI.Services.Constants;
using Microsoft.Extensions.Configuration;
using System.Text.Json;


namespace HackerAPI.Services
{
    public class StoryService : IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<StoryService> _logger;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncRateLimitPolicy _rateLimitPolicy;
        private readonly ApiSettings _apiSettings;

        public StoryService(IMemoryCache cache, ILogger<StoryService> logger, HttpClient httpClient)
        {
            _cache = cache;
            _logger = logger;
            _httpClient = httpClient;

            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            var resiliencySettings = configuration.GetSection("Resiliency").Get<ResiliencySettings>();
            _apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();

            _httpClient.BaseAddress = new Uri(_apiSettings.BaseUrl);

            _retryPolicy = Policy
              .Handle<HttpRequestException>()
              .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                  (exception, timeSpan, retryCount, context) =>
                  {
                      _logger.LogWarning($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                  });

            _rateLimitPolicy = Policy
              .RateLimitAsync(resiliencySettings.RateLimitRequests, TimeSpan.FromSeconds(resiliencySettings.RateLimitTimeWindowSeconds), resiliencySettings.RateLimitBurst); // Limit to 250 requests per 3 seconds with max burst of 200 requests at once
        }

        public async Task<IEnumerable<Story>> GetTopStoriesAsync(int count)
        {
            var storyIds = await GetStoriesIds();
            var stories = await GetDetailsForStories(storyIds.Take(count));
            return stories.OrderByDescending(x => x.Score);
        }

        public async Task<IEnumerable<int>?> GetStoriesIds()
        {
            return await _rateLimitPolicy.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(_apiSettings.BestStories);
                    _logger.LogInformation("Fetching top stories from Hacker News API");

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return JsonSerializer.Deserialize<List<int>>(result);
                    }
                    else
                    {
                        _logger.LogError($"Error: {response.StatusCode}, {response.RequestMessage}");
                        throw new HttpRequestException(response.Content.ToString());
                    }
                });
            });
        }

        public async Task<IEnumerable<Story>> GetDetailsForStories(IEnumerable<int> storyIds)
        {
            List<Task<Story>> storiesTasks = new List<Task<Story>>();

            foreach (int id in storyIds)
            {
                storiesTasks.Add(GetStoryDetailsById(id));
            }

            Story[] stories = await Task.WhenAll(storiesTasks);
            return stories.ToList();
        }

        public async Task<Story> GetStoryDetailsById(int id)
        {
            if (_cache.TryGetValue(id, out Story story))
            {
                _logger.LogInformation($"Fetching story with id {id} from cache");
                return story;
            }

            return await _rateLimitPolicy.ExecuteAsync(async () =>
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    _logger.LogInformation($"Fetching story with id {id} from Hacker News API");
                    HttpResponseMessage response = await _httpClient.GetAsync(String.Format(_apiSettings.Item, id));
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        story = JsonSerializer.Deserialize<StoryDto>(result).MapToObject();
                        _cache.Set(id, story, _cacheExpiration);
                        return story;
                    }
                    else
                    {
                        _logger.LogError($"Error: {response.StatusCode}, {response.RequestMessage}");
                        throw new HttpRequestException(response.Content.ToString());
                    }
                });
            });
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
