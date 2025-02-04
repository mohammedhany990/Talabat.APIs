using Talabat.Core.Entities.Product;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandTypeSpec : BaseSpecifications<Product>
    {
        public ProductWithBrandTypeSpec(ProductSpecificationParameters? parameters)
            : base(P =>
                (!parameters.BrandId.HasValue || P.ProductBrandId == parameters.BrandId) &&
                (!parameters.CategoryId.HasValue || P.ProductCategoryId == parameters.CategoryId) &&
                (string.IsNullOrEmpty(parameters.search) || P.Name.ToLower().Contains(parameters.search.ToLower())) &&
                (string.IsNullOrEmpty(parameters.BrandName) || P.ProductBrand.Name.ToLower().Contains(parameters.BrandName.ToLower())) &&
                (string.IsNullOrEmpty(parameters.CategoryName) || P.ProductCategory.Name.ToLower().Contains(parameters.CategoryName.ToLower()))

                  )
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductCategory);

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
                ApplyOrderByAsc(P => P.Id);
            }

            ApplyPagination(parameters.PageSize * (parameters.PageIndex - 1), parameters.PageSize);
        }

        public ProductWithBrandTypeSpec(int id)
            : base(P => P.Id == id)
        {
            Includes.Add(P => P.ProductBrand);
            Includes.Add(P => P.ProductCategory);
        }
    }
}
