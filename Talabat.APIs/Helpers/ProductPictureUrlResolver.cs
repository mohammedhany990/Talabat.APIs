using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;

namespace Talabat.APIs.Helpers
{
    public class ProductPictureUrlResolver : IValueResolver<Product, ProductReturningDTO, string>
    {
        private readonly IConfiguration _configuration;

        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Product source, ProductReturningDTO destination, string destMember, ResolutionContext context)
        {
            return $"{_configuration["APIBaseUrl"]}{source.PictureUrl}";
        }
    }
}
