using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate Next ,ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = Next;
            _logger = logger;
            _env = env;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var Response = _env.IsDevelopment() ? new ApiExceptionResponse(500, ex.Message, ex.StackTrace.ToString())
                               : new ApiExceptionResponse(500);

                // JavaScript don't unnderstand bascalCase, so convert it into CamelCase
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                //Convert this object into String
                var JsonRsponse = JsonSerializer.Serialize(Response, options);

                await context.Response.WriteAsync(JsonRsponse);
            }
        }
    }
}
