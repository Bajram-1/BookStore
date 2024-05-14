using BookStore.BLL.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.BLL.Services.Singletons
{
    public class CacheService : ICacheService
    {
        private Dictionary<string, object> _cache = new Dictionary<string, object>();

        private static object _lock = new object();

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory)
        {
            lock (_lock)
            {
                if (_cache.ContainsKey(key))
                {
                    return (T)_cache[key];
                }
            }

            T value = await factory();

            lock (_lock)
            {
                if (!_cache.ContainsKey(key))
                {
                    _cache[key] = value;
                }
                return value;
            }
        }

        public async Task RemoveAsync(string key)
        {
            lock (_lock)
            {
                _cache.Remove(key);
            }

            await Task.CompletedTask;
        }
    }
}
