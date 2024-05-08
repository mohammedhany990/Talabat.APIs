using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles: Profile 
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductReturningDTO>()
                     .ForMember(P => P.ProductBrand, O => O.MapFrom(S => S.ProductBrand.Name))
                     .ForMember(P => P.ProductType, O => O.MapFrom(S => S.ProductType.Name))
                     .ForMember(d=> d.PictureUrl, O=>O.MapFrom<ProductPictureUrlResolver>());
        }
    }
}
