using Microsoft.AspNetCore.Mvc;
using HackerAPI.Services;
using System.ComponentModel.DataAnnotations;
using Polly.RateLimit;

namespace HackerAPI.Controllers
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
        public async Task<IActionResult> GetTopStories([Range(0, 200)] int count)
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
            catch (RateLimitRejectedException ex)
            {
                _logger.LogError($"Rate limit exceeded while fetching top {count} stories: {ex.Message}");
                return StatusCode(429, "Rate limit exceeded. Please try again later.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HTTP request error while fetching top {count} stories: {ex.Message}");
                return StatusCode(503, "Service unavailable. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error fetching top {count} stories: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }

        }
    }
}
