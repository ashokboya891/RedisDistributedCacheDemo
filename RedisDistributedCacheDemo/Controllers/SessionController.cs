using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RedisDistributedCacheDemo.Models;

namespace RedisDistributedCacheDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public SessionController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserSession userSession)
        {
            // Generate a unique session key
            string sessionKey = $"Session_{userSession.Username}";

            // Serialize user session data to JSON
            string sessionData = JsonConvert.SerializeObject(userSession);

            // Store in Redis with expiration
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Session expires in 30 minutes
            };

            await _cache.SetStringAsync(sessionKey, sessionData, options);

            return Ok($"Session created for {userSession.Username}");
        }

        [HttpGet("get-session/{username}")]
        public async Task<IActionResult> GetSession(string username)
        {
            // Retrieve session data from Redis
            string sessionKey = $"Session_{username}";
            var sessionData = await _cache.GetStringAsync(sessionKey);

            if (sessionData == null)
            {
                return NotFound("Session not found or expired");
            }

            // Deserialize JSON back to object
            var userSession = JsonConvert.DeserializeObject<UserSession>(sessionData);

            return Ok(userSession);
        }

        [HttpPost("logout/{username}")]
        public async Task<IActionResult> Logout(string username)
        {
            // Remove the session from Redis
            string sessionKey = $"Session_{username}";
            await _cache.RemoveAsync(sessionKey);

            return Ok($"Session for {username} has been removed");
        }
    }
}
