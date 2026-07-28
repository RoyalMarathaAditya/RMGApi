namespace HRMS.Api.Services.Interfaces
{
    public class CacheEntryOptions
    {
        public TimeSpan DistributedExpiration { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan MemoryExpiration { get; set; } = TimeSpan.FromMinutes(2);
    }

    public interface ICacheService
    {
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, CacheEntryOptions? options = null) where T : class;
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
    }
}
