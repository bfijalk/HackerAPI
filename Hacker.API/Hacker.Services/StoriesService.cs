using Hacker.Models;
using Hacker.Models.Common;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Hacker.Services
{
    public class StoriesService : IStoriesService
    {
        private HttpClient _httpClient;
        private readonly ILogger<IStoriesService> _logger;

        public StoriesService(HttpClient client, ILogger<IStoriesService> logger) 
        {
            _httpClient = client;
            _logger = logger;
        }

        /// <summary>
        /// Get id's of top 200 stories from hacker API
        /// </summary>
        /// <returns></returns>
        public async ValueTask<IEnumerable<int>> GetStoriesIds() 
        {
            _logger.LogInformation("Fetching top stories ID's from hacker API");

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(Constants.BEST_STORIES_URL);
                return await JsonSerializer.DeserializeAsync<List<int>>(await response.Content.ReadAsStreamAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while fetching top stories from hacker API", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get determined top stories with details from hacker API
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async ValueTask<IEnumerable<Story>> GetTopStories(int count)
        {
            var topStories = await GetStoriesIds();
            var topStoriesDetails = new ConcurrentBag<Story>();

            Task[] tasks = new Task[count];

            foreach(var storyId in topStories.Take(count)) 
            {
                count--;
                tasks[count] = Task.Run(async () =>
                {
                    var storyItem = await GrabStoryById(storyId);
                    if (storyItem != null)
                        topStoriesDetails.Add(storyItem);             
                });
            }
        
            Task.WaitAll(tasks);
            return topStoriesDetails.OrderByDescending(x => x.Score);
        }

        /// <summary>
        /// Fetch single story details by its ID
        /// </summary>
        /// <param name="storyId"></param>
        /// <returns></returns>
        public async Task<Story?> GrabStoryById(int storyId)
        {
            _logger.LogInformation($"Fetching details for story with id {storyId}");

            var requestUrl = ConstructStoryDetailsUrl(storyId);
            try
            {
                var item = await _httpClient.GetAsync(requestUrl);
                var storyItem = await JsonSerializer.DeserializeAsync<StoryDto>(await item.Content.ReadAsStreamAsync());
                return storyItem.MapToObject();
            }catch (Exception ex) 
            {
                _logger.LogError($"Failed to fetch details for story with id {storyId}", ex.Message);
                throw;
            }
        }

        private string ConstructStoryDetailsUrl(int storyId) 
        {
            return Constants.PREFIX_STORY_DETAILS + storyId + ".json";
        }
    }
}
