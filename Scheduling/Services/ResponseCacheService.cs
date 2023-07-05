
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Scheduling.Services
{
    public interface IResponseCacheService
    {
        Task<T> GetFromCache<T>(string key) where T : class;
        Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        Task ClearCache(string key);
    }
    public class ResponseCacheService:IResponseCacheService
    {
        private readonly IDistributedCache _cache;

        public ResponseCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedUsers = await _cache.GetStringAsync(key);
            return cachedUsers == null ? null : JsonSerializer.Deserialize<T>(cachedUsers);
        }

        public async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            var users = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, users, options);
        }

        public async Task ClearCache(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
