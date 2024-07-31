namespace Talabat.Core.Services
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string key, object value, TimeSpan spiry);
        Task<string?> GetCacheResponseAsync(string key);
    }
}
