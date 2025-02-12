using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisDistributedCacheDemo.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public CacheController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet("set")]
        public async Task<IActionResult> SetCache()
        {
            const string cacheKey = "ExampleKey";
            var cacheValue = "Hello,Ashok Redis!";

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) // Cache expires in 5 minutes
            };

            await _cache.SetStringAsync(cacheKey, cacheValue, options);

            return Ok("Cache set successfully");
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetCache()
        {
            const string cacheKey = "ExampleKey";
            var cacheValue = await _cache.GetStringAsync(cacheKey);

            if (cacheValue == null)
            {
                return NotFound("Cache not found");
            }

            return Ok(cacheValue);
        }
    }
}
