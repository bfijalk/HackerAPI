using Hacker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacker.Tests
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
                    Score = 100,
                    Descendants= 30,
                    By = "Rikki",
                    Title = "First test story",
                    Url = "Url",
                    Time = 300
                },
                new StoryDto()
                {
                    Score = 300,
                    Descendants= 50,
                    By = "Robert",
                    Title = "Second test story",
                    Url = "Url",
                    Time = 300
                },
                new StoryDto()
                {
                    Score = 120,
                    Descendants= 90,
                    By = "Tom",
                    Title = "Third test story",
                    Url = "Url",
                    Time = 300
                },
                new StoryDto()
                {
                    Score = 101,
                    Descendants= 37,
                    By = "Jason",
                    Title = "Fourth test story",
                    Url = "Url",
                    Time = 300
                },
            ];
        }
    }
}
