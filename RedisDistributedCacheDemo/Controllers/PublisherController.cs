using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisDistributedCacheDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;

        public PublisherController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [HttpPost]
        [Route("publish")]
        public async Task<IActionResult> Publish([FromQuery] string channel, [FromBody] string message)
        {
            var subscriber = _redis.GetSubscriber();
            await subscriber.PublishAsync(channel, message);
            return Ok(new { Channel = channel, Message = message });
        }
    }
}
