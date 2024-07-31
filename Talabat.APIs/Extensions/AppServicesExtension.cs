using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helper;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service.CacheService;
using Talabat.Service.OrderService;
using Talabat.Service.PaymentService;

namespace Talabat.APIs.Extensions
{
    public static class AppServicesExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection Services)
        {

            // To Allow DI for IBasketRepo
            Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

            // To Allow DI for IUnitOfWork
            Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Allow DI for AutoMapper
            Services.AddAutoMapper(typeof(MappingProfiles));

            // Allow DI for IOrderService
            Services.AddScoped<IOrderService, OrderService>();

            // Allow DI for IPaymentService
            Services.AddScoped<IPaymentService, PaymentService>();

            Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

            // Validation Error: api/id {id is int}-> api/five will generate this error
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(P => P.Value.Errors.Count() > 0)
                        .SelectMany(P => P.Value.Errors)
                        .Select(P => P.ErrorMessage)
                        .ToArray();

                    var response = new ValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);

                };
            });

            return Services;
        }
    }
}
