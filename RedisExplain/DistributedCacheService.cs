
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;

namespace RedisExplain
{
    public class DistributedCacheService : ICacheService
    {
        IDistributedCache _distributedCache;
        public DistributedCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T> GetCacheValueAsync<T>(string key)
        {
            var jsonStr = await _distributedCache.GetStringAsync(key);
            if (jsonStr == null)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public Task SetCacheValueAsync<T>(string key, T value)
        {
            var jsonStr = JsonConvert.SerializeObject(value);
            return _distributedCache.SetStringAsync(key, jsonStr);
        }
    }
}
