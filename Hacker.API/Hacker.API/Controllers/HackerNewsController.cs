using Hacker.Models;
using Hacker.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Hacker.API.Controllers
{
    [ApiController]
    [Route("HackerNews")]
    public class HackerNewsController : Controller
    {
        private readonly IStoriesService _storiesService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<HackerNewsController> _logger;

        public HackerNewsController(IStoriesService storiesService, ICacheService cacheService, ILogger<HackerNewsController> logger)
        {
            _storiesService = storiesService;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet(Name = "GetTopStories")]
        public async Task<IActionResult> GetStoriesAsync([Range(0, 200)]int count)
        {
            _logger.LogInformation($"Fetching top {count} stories from Hacker News API");
            IEnumerable<Story>? stories;

            stories = _cacheService.GetFromCache<IEnumerable<Story>>(count);
            if (stories!=null)
            {
                _logger.LogInformation($"Returning objects: {JsonSerializer.Serialize(stories, new JsonSerializerOptions() { WriteIndented = true })}");
                return Ok(stories);
            }

            try
            {
                stories = await _storiesService.GetTopStories(count);           
            }
            catch(Exception ex) 
            {
                _logger.LogError($"Fetching top {count} stories failed", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return CheckResults(stories, count);
        }

        private IActionResult CheckResults(IEnumerable<Story> stories, int count) 
        {

            if (stories.Count() == count)
            {
                _logger.LogInformation($"Top {count} stories fetched successfully");
                _logger.LogInformation($"Returning objects: {JsonSerializer.Serialize(stories, new JsonSerializerOptions() { WriteIndented = true })}");
                _cacheService.AddToCache(count, stories);
                return Ok(stories);
            }
            else
            {
                _logger.LogWarning($"All requests were completed successfully, but total number of stories fetched from API {(stories.Count())} is different than {count}");
                return NoContent();
            }
        }

    }
}
