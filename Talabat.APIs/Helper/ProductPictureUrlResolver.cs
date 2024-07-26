using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities.Product;

namespace Talabat.APIs.Helper
{

    public class ProductPictureUrlResolver : IValueResolver<Product, ProductReturningDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Product source,
            ProductReturningDto destination,
            string destMember,
            ResolutionContext context
        )
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
                return $"{_configuration["APIBaseUrl"]}{source.PictureUrl}";

            return string.Empty;
        }
    }


}
