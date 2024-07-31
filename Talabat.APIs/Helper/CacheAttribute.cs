using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helper
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _liveTimeInSec;

        public CacheAttribute(int liveTimeInSec)
        {
            _liveTimeInSec = liveTimeInSec;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // DI Explicitly 
            var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var response = await responseCacheService.GetCacheResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(response))
            {
                var result = new ContentResult()
                {
                    Content = response,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = result;
                return;
            }

            var actionExecutedContext = await next.Invoke();
            if (actionExecutedContext.Result is ObjectResult okObjectResult && okObjectResult.Value is not null)
            {
                await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_liveTimeInSec));
            }

        }

        private string GenerateCacheKeyFromRequest(HttpRequest httpContextRequest)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(httpContextRequest.Path);
            foreach (var (key, value) in httpContextRequest.Query.OrderBy(k => k.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
