using Microsoft.AspNetCore.Mvc;
using SantanderTest.HackerAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace SantanderTest.HackerAPI.Controllers
{
    [ApiController]
    [Route("HackerAPI")]
    public class HackerController : Controller
    {
        private readonly StoryService _storyService;
        private readonly ILogger<HackerController> _logger;

        public HackerController(StoryService storyService, ILogger<HackerController> logger)
        {
            _storyService = storyService;
            _logger = logger;
        }

        [HttpGet(Name = "GetStories")]
        public async Task<IActionResult> GetTopStories([Range(0,500)] int count)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Fetching top {count} stories from Hacker News API");
            try
            {
                var result = await _storyService.GetTopStoriesAsync(count);
                _logger.LogInformation($"Successfully fetched top {result.Count()} stories from API");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching top {count} stories from API: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
