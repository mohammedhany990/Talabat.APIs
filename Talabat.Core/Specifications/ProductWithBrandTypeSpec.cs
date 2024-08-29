using Talabat.Core.Entities.Product;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandTypeSpec : BaseSpecifications<Product>
    {
        public ProductWithBrandTypeSpec(ProductSpecificationParameters? parameters)
            : base(P =>
                (!parameters.BrandId.HasValue || P.ProductBrandId == parameters.BrandId) &&
                (!parameters.TypeId.HasValue || P.ProductTypeId == parameters.TypeId) &&
                (string.IsNullOrEmpty(parameters.search) || P.Name.ToLower().Contains(parameters.search.ToLower())) &&
                (string.IsNullOrEmpty(parameters.BrandName) || P.ProductBrand.Name.ToLower().Contains(parameters.BrandName.ToLower())) &&
                (string.IsNullOrEmpty(parameters.TypeName) || P.ProductType.Name.ToLower().Contains(parameters.TypeName.ToLower()))

                  )
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);

            if (!string.IsNullOrEmpty(parameters.sort))
            {
                switch (parameters.sort)
                {
                    case "priceAsc":
                        ApplyOrderByAsc(P => P.Price);
                        break;

                    case "priceDesc":
                        ApplyOrderByDesc(P => P.Price);
                        break;

                    case "nameDesc":
                        ApplyOrderByDesc(P => P.Name);
                        break;

                }
            }
            else
            {
                ApplyOrderByAsc(P => P.Name);
            }

            ApplyPagination(parameters.PageSize * (parameters.PageIndex - 1), parameters.PageSize);
        }

        public ProductWithBrandTypeSpec(int id)
            : base(P => P.Id == id)
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductType);
        }
    }
}
