using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisDistributedCacheDemo.IServices;

namespace RedisDistributedCacheDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IRedisService _redisService;

        public SessionController(IRedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession(string sessionId, string username)
        {
            var sessionData = $"Session for {username} created at {DateTime.UtcNow}";
            await _redisService.SetDataAsync(sessionId, sessionData, TimeSpan.FromMinutes(15));
            return Ok(new { message = "Session created successfully!", sessionId });
        }

        [HttpGet("get-session")]
        public async Task<IActionResult> GetSession(string sessionId)
        {
            var sessionData = await _redisService.GetDataAsync(sessionId);

            if (sessionData == null)
            {
                return NotFound(new { message = "Session not found or expired." });
            }

            // Refresh session expiration (Sliding Expiration)
            await _redisService.SetDataAsync(sessionId, sessionData, TimeSpan.FromMinutes(15));

            return Ok(new { sessionData });
        }

        [HttpDelete("delete-session")]
        public async Task<IActionResult> DeleteSession(string sessionId)
        {
            await _redisService.RemoveDataAsync(sessionId);
            return Ok(new { message = "Session deleted successfully!" });
        }

        [HttpGet("rate-limit")]
        public async Task<IActionResult> RateLimit(string userId)
        {
            var rateLimitKey = $"RateLimit_{userId}";
            var requestCount = await _redisService.GetDataAsync(rateLimitKey);

            if (requestCount == null)
            {
                // First request in the time window
                await _redisService.SetDataAsync(rateLimitKey, "1", TimeSpan.FromMinutes(1));
            }
            else if (int.Parse(requestCount) >= 5)
            {
                // Exceeded request limit
                return StatusCode(429, new { message = "Too many requests. Please wait and try again." });
            }
            else
            {
                // Increment request count
                var newCount = (int.Parse(requestCount) + 1).ToString();
                await _redisService.SetDataAsync(rateLimitKey,newCount, TimeSpan.FromMinutes(1));
            }

            return Ok(new { message = "Request allowed!" });
        }
    }
}
