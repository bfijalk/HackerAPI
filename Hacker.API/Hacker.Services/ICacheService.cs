using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacker.Services
{
    public interface ICacheService
    {
        public T GetFromCache<T>(int key);

        public void AddToCache<T>(int count, T records);
    }
}
