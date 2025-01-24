using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantanderTest.HackerAPI.Services.Constants
{
    public static class ApiUris
    {
        public const string BASE_URI = "https://hacker-news.firebaseio.com/v0/";
        public const string BEST_STORIES = "beststories.json";
        public const string ITEM = "item/{0}.json";
    }
}
