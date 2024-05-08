using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Repositories;
using Talabat.Repository;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            //To Allow DI 
            //builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            // To Allow DI for any class implement IGenericRepository 
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //To Allow Di for AutoMapper
            Services.AddAutoMapper(typeof(ProductReturningDTO));

            // ValidationError
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var erros = actionContext.ModelState.Where(E => E.Value.Errors.Count() > 0)
                                            .SelectMany(E => E.Value.Errors)
                                            .Select(E => E.ErrorMessage)
                                            .ToArray();

                    var validationErrorResponse = new ValidationErrorResponse()
                    {
                        Errors = erros
                    };
                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            return Services;
        }
    }
}
