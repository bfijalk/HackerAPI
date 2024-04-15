using Hacker.Models;
using Hacker.Models.Common;
using Hacker.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using System.Text.Json;

namespace Hacker.Tests
{
    public class StoriesServiceTests
    {
        private Mock<HttpMessageHandler> _httpHandlerMock;
        private Mock<ILogger<IStoriesService>> _loggerMock;
        private IStoriesService _storiesService;

        [SetUp]
        public void Setup()
        {
            _httpHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<IStoriesService>>();
            _storiesService = new StoriesService(new HttpClient(_httpHandlerMock.Object), _loggerMock.Object);   
        }

        [Test]
        public async Task GetStoriesIds_Successfully()
        {
            //Arrange
            HttpGetIdsResponseMock();

            //Act
            var result = await _storiesService.GetStoriesIds();

            //Assert
            CollectionAssert.AreEqual(result, TestDataProvider.GetIds()); 
        }

        [Test]
        public async Task GrabStoryById_Successfully()
        {
            //Arrange
            var expectedObject = GrabObjectByIdMock(1);

            //Act
            var result = await _storiesService.GrabStoryById(1);

            //Assert
            Assert.That(result.Title, Is.EqualTo(expectedObject.MapToObject().Title));
            Assert.That(result.Score, Is.EqualTo(expectedObject.MapToObject().Score));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task GetTopStories_Successfully(int count)
        {
            //Arrange
            HttpGetIdsResponseMock();
            GrabObjectByIdMock(0);
            GrabObjectByIdMock(1);
            GrabObjectByIdMock(2);
            GrabObjectByIdMock(3);

            //Act
            var result = await _storiesService.GetTopStories(count);

            //Assert
            Assert.That(result.Count(), Is.EqualTo(count));
        }

        private void HttpGetIdsResponseMock()
        {
            var stories = JsonSerializer.Serialize(TestDataProvider.GetIds());

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(stories),
                StatusCode = System.Net.HttpStatusCode.OK
            };
            _httpHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(request => request.RequestUri == new Uri(Constants.BEST_STORIES_URL)),
                ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(response);
        }

        private StoryDto GrabObjectByIdMock(int id)
        {
            var expectedObject = TestDataProvider.GetEntries().ToList();
            var stories = JsonSerializer.Serialize(expectedObject[id]);
            var url = Constants.PREFIX_STORY_DETAILS + id + ".json";

            var response = new HttpResponseMessage()
            {
                Content = new StringContent(stories),
                StatusCode = System.Net.HttpStatusCode.OK
            };
            _httpHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(request => request.RequestUri.ToString() == url),
                ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(response);

            return expectedObject[id];
        }
    }
}