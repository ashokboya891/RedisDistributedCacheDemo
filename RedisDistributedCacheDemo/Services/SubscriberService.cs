using StackExchange.Redis;

namespace RedisDistributedCacheDemo.Services
{
    public class SubscriberService:BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;

        public SubscriberService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redis.GetSubscriber();
            await subscriber.SubscribeAsync("my_channel", (channel, message) =>
            {
                Console.WriteLine($"Received message: {message} on channel: {channel}");
            });
        }

    }
}
