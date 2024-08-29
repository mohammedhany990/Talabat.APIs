namespace Talabat.Core.Services
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string key, object? value, TimeSpan expire);
        Task<string?> GetCacheResponseAsync(string key);
    }
}
