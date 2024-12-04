using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CachingAbstractionsinASP.Extensions
{
    //Here's how you can implement the cache-aside pattern as an extension method for IDistributedCache:
    public static class DistributedCacheExtensions
    {
        public static DistributedCacheEntryOptions DefaultExpiration => new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
        };

        public static async Task<T> GetOrCreateAsync<T>(
            this IDistributedCache cache,
            string key,
            Func<Task<T>> factory,
            DistributedCacheEntryOptions? cacheOptions = null)
        {
            var cachedData = await cache.GetStringAsync(key);

            if (cachedData is not null)
            {
                return JsonSerializer.Deserialize<T>(cachedData);
            }

            var data = await factory();

            await cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(data),
                cacheOptions ?? DefaultExpiration);

            return data;
        }
    }
}
