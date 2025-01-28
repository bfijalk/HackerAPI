using HackerAPI.Model;

namespace HackerAPI.Tests
{
    public class TestDataProvider
    {
        public static IEnumerable<int> GetIds()
        {
            return [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        }

        public static IEnumerable<StoryDto> GetEntries()
        {
            return
            [
                new StoryDto()
                {   
                    Id = 1,
                    Score = 100,
                    Descendants= 30,
                    PostedBy = "Rikki",
                    Title = "First test story",
                    Uri = "Url",
                    Time = DateTimeOffset.FromUnixTimeSeconds(30)
                },
                new StoryDto()
                {
                    Id = 2,
                    Score = 300,
                    Descendants= 50,
                    PostedBy = "Robert",
                    Title = "Second test story",
                    Uri = "Url",
                    Time = DateTimeOffset.FromUnixTimeSeconds(30)
                },
                new StoryDto()
                {
                    Id = 3,
                    Score = 120,
                    Descendants= 90,
                    PostedBy = "Tom",
                    Title = "Third test story",
                    Uri = "Url",
                    Time = DateTimeOffset.FromUnixTimeSeconds(30)
                },
                new StoryDto()
                {
                    Id = 4, 
                    Score = 101,
                    Descendants= 37,
                    PostedBy = "Jason",
                    Title = "Fourth test story",
                    Uri = "Url",
                    Time = DateTimeOffset.FromUnixTimeSeconds(30)
                },
            ];
        }
    }
}
