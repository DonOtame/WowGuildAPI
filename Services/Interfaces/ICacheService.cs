namespace WowGuildAPI.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string cacheKey, Func<Task<T>> createItem, TimeSpan? expiration = null);
    }
}