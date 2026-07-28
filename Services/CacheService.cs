using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using HRMS.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace HRMS.Api.Services
{
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _distributed;
        private readonly IMemoryCache _memory;
        private readonly ILogger<DistributedCacheService> _logger;

        private static readonly ConcurrentDictionary<string, byte> TrackedKeys = new(StringComparer.OrdinalIgnoreCase);

        public DistributedCacheService(IDistributedCache distributed, IMemoryCache memory, ILogger<DistributedCacheService> logger)
        {
            _distributed = distributed;
            _memory = memory;
            _logger = logger;
        }

        private string NormalizeKey(string key)
        {
            return $"HRMS:{key}";
        }

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, CacheEntryOptions? options = null) where T : class
        {
            options ??= new CacheEntryOptions();
            var cacheKey = NormalizeKey(key);

            if (_memory.TryGetValue(cacheKey, out T? cached) && cached != null)
                return cached;

            try
            {
                var distributed = await _distributed.GetStringAsync(cacheKey);
                if (distributed != null)
                {
                    var result = JsonSerializer.Deserialize<T>(distributed);
                    if (result != null)
                    {
                        _memory.Set(cacheKey, result, options.MemoryExpiration);
                        TrackedKeys.TryAdd(cacheKey, 0);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Distributed cache GET failed for {CacheKey}", cacheKey);
            }

            var sw = Stopwatch.StartNew();
            var fresh = await factory();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 100)
                _logger.LogWarning("Slow factory for {CacheKey} took {ElapsedMs}ms", cacheKey, sw.ElapsedMilliseconds);

            try
            {
                var serialized = JsonSerializer.Serialize(fresh);
                await _distributed.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = options.DistributedExpiration
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Distributed cache SET failed for {CacheKey}", cacheKey);
            }

            _memory.Set(cacheKey, fresh, options.MemoryExpiration);
            TrackedKeys.TryAdd(cacheKey, 0);

            return fresh;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var cacheKey = NormalizeKey(key);

            if (_memory.TryGetValue(cacheKey, out T? cached) && cached != null)
                return cached;

            try
            {
                var distributed = await _distributed.GetStringAsync(cacheKey);
                if (distributed != null)
                {
                    var result = JsonSerializer.Deserialize<T>(distributed);
                    if (result != null)
                    {
                        _memory.Set(cacheKey, result, TimeSpan.FromMinutes(2));
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Distributed cache GET failed for {CacheKey}", cacheKey);
            }

            return null;
        }

        public async Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null) where T : class
        {
            options ??= new CacheEntryOptions();
            var cacheKey = NormalizeKey(key);

            try
            {
                var serialized = JsonSerializer.Serialize(value);
                await _distributed.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = options.DistributedExpiration
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Distributed cache SET failed for {CacheKey}", cacheKey);
            }

            _memory.Set(cacheKey, value, options.MemoryExpiration);
            TrackedKeys.TryAdd(cacheKey, 0);
        }

        public async Task RemoveAsync(string key)
        {
            var cacheKey = NormalizeKey(key);
            _memory.Remove(cacheKey);
            try
            {
                await _distributed.RemoveAsync(cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Distributed cache REMOVE failed for {CacheKey}", cacheKey);
            }
            TrackedKeys.TryRemove(cacheKey, out _);
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            var cachePrefix = NormalizeKey(prefix);
            var keysToRemove = TrackedKeys.Keys
                .Where(k => k.StartsWith(cachePrefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _memory.Remove(key);
                TrackedKeys.TryRemove(key, out _);
            }

            _logger.LogInformation("Cache invalidated for prefix {Prefix} - removed {Count} entries", prefix, keysToRemove.Count);
            return Task.CompletedTask;
        }
    }
}
