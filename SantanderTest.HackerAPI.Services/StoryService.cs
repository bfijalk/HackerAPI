using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SantanderTest.HackerAPI.Model;
using SantanderTest.HackerAPI.Services.Constants;
using System.Text.Json;

namespace SantanderTest.HackerAPI.Services
{
    public class StoryService : IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<StoryService> _logger;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

        public StoryService(IMemoryCache cache, ILogger<StoryService> logger, HttpClient httpClient)
        {
            _cache = cache;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(ApiUris.BASE_URI);
        }

        public async Task<IEnumerable<Story>> GetTopStoriesAsync(int count)
        {
            var storyIds = await GetStoriesIds();
            var stories = await GetDetailsForStories(storyIds.Take(count));
            return stories.OrderByDescending(x => x.Score);
        }

        public async Task<IEnumerable<int>> GetStoriesIds()
        {

            HttpResponseMessage response = await _httpClient.GetAsync(ApiUris.BEST_STORIES);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<int>>(result);
            }
            else
            {
                _logger.LogError($"Error: {response.StatusCode}, {response.RequestMessage}");
                return new List<int>();
            }

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
                return story;
            }

            HttpResponseMessage response = await _httpClient.GetAsync(String.Format(ApiUris.ITEM, id));
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
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
