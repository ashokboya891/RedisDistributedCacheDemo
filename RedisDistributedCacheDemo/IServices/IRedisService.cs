namespace RedisDistributedCacheDemo.IServices
{
    public interface IRedisService
    {
        Task SetDataAsync(string key, string value, TimeSpan? expiration = null);
        Task<string?> GetDataAsync(string key);
        Task<bool> KeyExistsAsync(string key);
        Task RemoveDataAsync(string key);
    }
}
