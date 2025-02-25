using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using WowGuildAPI.Services.Interfaces;

namespace WowGuildAPI.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> createItem, TimeSpan? expiration = null)
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonConvert.DeserializeObject<T>(cachedData);
            }   

            var data = await createItem();
            if (data != null)
            {
                var serializedData = JsonConvert.SerializeObject(data);
                await _cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
                });
            }

            return data;
        }
    }
}