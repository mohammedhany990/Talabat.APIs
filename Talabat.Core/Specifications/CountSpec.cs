using Talabat.Core.Entities.Product;

namespace Talabat.Core.Specifications
{
    public class CountSpec : BaseSpecifications<Product>
    {
        public CountSpec(ProductSpecificationParameters? parameters)
            : base(P =>

                (!parameters.BrandId.HasValue || P.ProductBrandId == parameters.BrandId) &&
                (!parameters.TypeId.HasValue || P.ProductTypeId == parameters.TypeId) &&
                (string.IsNullOrEmpty(parameters.search) || P.Name.ToLower().Contains(parameters.search.ToLower())) &&
                (string.IsNullOrEmpty(parameters.BrandName) || P.ProductBrand.Name.ToLower().Contains(parameters.BrandName.ToLower())) &&
                (string.IsNullOrEmpty(parameters.TypeName) || P.ProductType.Name.ToLower().Contains(parameters.TypeName.ToLower()))


            )
        {

        }
    }
}
