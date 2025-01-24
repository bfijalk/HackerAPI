using Microsoft.Extensions.Caching.Memory;
using Moq;
using SantanderTest.HackerAPI.Model;
using SantanderTest.HackerAPI.Services;
using SantanderTest.HackerAPI.Services.Constants;
using Microsoft.Extensions.Logging; 
using Moq.Protected;
using System.Text.Json;
using FluentAssertions;

namespace SantanderTest.HackerAPI.Tests
{
    [TestFixture]
    public class StoriesServiceTests
    {
        private Mock<IMemoryCache> _mockCache;
        private Mock<ILogger<StoryService>> _mockLogger;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private HttpClient _httpClient;
        private StoryService _storyService;

        [SetUp]
        public void SetUp()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<StoryService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(ApiUris.BASE_URI)
            };
            _storyService = new StoryService(_mockCache.Object, _mockLogger.Object, _httpClient);
            HttpGetIdsResponseMock();
        }

        [Test]
        public async Task GetStoriesIds_ShouldReturnStoriesIds()
        {
            //Arrange

            //Act
            var result = await _storyService.GetStoriesIds();

            //Assert
            CollectionAssert.AreEqual(result, TestDataProvider.GetIds());
        }

        [Test]
        public async Task GetStoryDetailsById_ShouldReturnStory()
        {
            //Arrange
            var expectedObject = GrabObjectDetailsByIdMock(1);
            _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            //Act
            var result = await _storyService.GetStoryDetailsById(1);

            //Assert
            Assert.That(result.Title, Is.EqualTo(expectedObject.MapToObject().Title));
            Assert.That(result.Score, Is.EqualTo(expectedObject.MapToObject().Score));
            Assert.That(result.CommentCount, Is.EqualTo(expectedObject.MapToObject().CommentCount));
            Assert.That(result.PostedBy, Is.EqualTo(expectedObject.MapToObject().PostedBy));
        }

        [Test]
        public async Task GetStoryDetailsById_ShouldReturnStory_WhenCacheIsHit()
        {
            // Arrange
            var storyId = 1;
            var expectedStory = GrabObjectDetailsByIdMock(storyId).MapToObject();
            MockSingleStoryInCache(storyId);

            // Act
            await _storyService.GetStoryDetailsById(storyId);
            await _storyService.GetStoryDetailsById(storyId);
            var result = await _storyService.GetStoryDetailsById(storyId);

            // Assert
            result.Should().BeEquivalentTo(expectedStory);
        }



        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetTopStories_ShouldReturnListOfStories(int count)
        {
            //Arrange
            var expectedObject = TestDataProvider.GetEntries().ToList();
            var expectedList = TestDataProvider.GetEntries()
                .Select(x => x.MapToObject())
                .OrderByDescending(x => x.Score)
                .Take(count)
                .ToList();
            _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>()); ;

            GrabObjectDetailsByIdMock(0);
            GrabObjectDetailsByIdMock(1);
            GrabObjectDetailsByIdMock(2);
            GrabObjectDetailsByIdMock(3);

            //Act
            var result = await _storyService.GetTopStoriesAsync(count);

            //Assert
            result.Should().BeEquivalentTo(expectedList, options => options
                .ComparingByMembers<Story>()
                .WithStrictOrdering());
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetTopStories_ShouldReturnListOfStories_WhenCacheIsHit(int count)
        {
            //Arrange
            var expectedObject = TestDataProvider.GetEntries().ToList();
            var expectedList = TestDataProvider.GetEntries()
                .Select(x => x.MapToObject())
                .OrderByDescending(x => x.Score)
                .Take(count)
                .ToList();
 
            MockSingleStoryInCache(0);
            MockSingleStoryInCache(1);
            MockSingleStoryInCache(2);
            MockSingleStoryInCache(3);

            //Act
            await _storyService.GetTopStoriesAsync(count);
            await _storyService.GetTopStoriesAsync(count);
            var result = await _storyService.GetTopStoriesAsync(count);

            //Assert
            result.Should().BeEquivalentTo(expectedList, options => options
                .ComparingByMembers<Story>()
                .WithStrictOrdering());
        }

        private void MockSingleStoryInCache(int id)
        {
            object expectedObject = GrabObjectDetailsByIdMock(id);
            _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out expectedObject)).Returns(true);
        }

        private void HttpGetIdsResponseMock()
        {
            var stories = JsonSerializer.Serialize(TestDataProvider.GetIds());

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(stories),
                StatusCode = System.Net.HttpStatusCode.OK
            };
            _mockHttpMessageHandler
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(request => request.RequestUri == new Uri(ApiUris.BASE_URI + ApiUris.BEST_STORIES)),
                ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(response);
        }


        private StoryDto GrabObjectDetailsByIdMock(int id)
        {
            var expectedObject = TestDataProvider.GetEntries().ToList();
            var stories = JsonSerializer.Serialize(expectedObject[id]);
            var url = ApiUris.BASE_URI + String.Format(ApiUris.ITEM, id);

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(stories),
                StatusCode = System.Net.HttpStatusCode.OK
            };
            _mockHttpMessageHandler
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(request => request.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(response);

            return expectedObject[id];
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
            _storyService.Dispose();
        }
    }
}