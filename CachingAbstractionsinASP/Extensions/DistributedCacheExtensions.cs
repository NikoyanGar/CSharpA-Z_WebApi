using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace CachingAbstractionsinASP.Extensions
{
    //Here's how you can implement the cache-aside pattern as an extension method for IDistributedCache:
    //    Challenges:
    //Cache Invalidation: Ensuring the cache remains up-to-date when the underlying data changes.
    //Stale Data: There's a risk of serving outdated data if the cache is not properly invalidated or updated.
    //Complexity: Application logic needs to handle cache misses, data retrieval, and updates.
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
