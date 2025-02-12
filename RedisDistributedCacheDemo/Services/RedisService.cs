using Microsoft.Extensions.Caching.Distributed;
using RedisDistributedCacheDemo.IServices;

namespace RedisDistributedCacheDemo.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetDataAsync(string key, string value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                //AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10), // absexp
               AbsoluteExpirationRelativeToNow = expiration,
                SlidingExpiration = expiration
            };
            await _cache.SetStringAsync(key, value, options);
        }

        public async Task<string?> GetDataAsync(string key)
        {
            return await _cache.GetStringAsync(key);
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            var data = await _cache.GetStringAsync(key);
            return data != null;
        }

        public async Task RemoveDataAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
