using Hacker.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hacker.Services
{
    public interface IStoriesService
    {
        public ValueTask<IEnumerable<int>> GetStoriesIds();
        public ValueTask<IEnumerable<Story>> GetTopStories(int count);
        public Task<Story?> GrabStoryById(int storyId);

    }
}
