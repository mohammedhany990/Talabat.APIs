using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.APIs.DTOs.Order;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Entities.Product;


namespace Talabat.APIs.Helper
{
    public class MappingProfiles : Profile
    {

        public MappingProfiles()
        {

            CreateMap<Product, ProductReturningDto>()
                .ForMember(M => M.Brand,
                    O => O.MapFrom(B => B.ProductBrand.Name))

                .ForMember(M => M.Category,
                    O => O.MapFrom(T => T.ProductType.Name))

                .ForMember(M => M.PictureUrl,
                    O => O.MapFrom<ProductPictureUrlResolver>());


            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();

            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<AddressDto, Core.Entities.Order_Aggregate.UserOrderAddress>();


            CreateMap<Order, OrderToReturnDto>()
                .ForMember(M => M.DeliveryMethod, D => D.MapFrom(N => N.DeliveryMethod.ShortName))
                .ForMember(M => M.Cost, I => I.MapFrom(C => C.DeliveryMethod.Cost));


            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductName, O => O.MapFrom(I => I.Product.ProductName))
                .ForMember(d => d.ProductId, O => O.MapFrom(I => I.Product.ProductId))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());

        }




    }
}
