using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacker.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ICacheService> _logger; 

        public CacheService(IMemoryCache memoryCache, ILogger<ICacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public T GetFromCache<T>(int key)
        {
            _logger.LogInformation($"Trying to get {key} records from cache");
            return (T)_memoryCache.Get(key);
        }
        public void AddToCache<T>(int count, T records)
        {
            _logger.LogInformation($"Adding {count} records to cache.");
            _memoryCache.Set(count, records,DateTimeOffset.Now.AddHours(3));
        }
    }
}
