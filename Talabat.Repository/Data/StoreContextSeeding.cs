using System.Text.Json;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Entities.Product;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeeding
    {
        public static async Task DataSeedAsync(TalabatDbContext dbContext)
        {

            if (dbContext.ProductBrands.Count() == 0)
            {
                var BrandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);
                if (Brands?.Count() > 0)
                {
                    foreach (var brand in Brands)
                    {
                        dbContext.Set<ProductBrand>().Add(brand);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }

            if (dbContext.ProductTypes.Count() == 0)
            {
                var CategoriesData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/types.json");
                var ProductTypes = JsonSerializer.Deserialize<List<ProductType>>(CategoriesData);
                if (ProductTypes?.Count() > 0)
                {
                    foreach (var type in ProductTypes)
                    {
                        dbContext.Set<ProductType>().Add(type);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }

            if (dbContext.Products.Count() == 0)
            {
                var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(ProductsData);

                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        dbContext.Set<Product>().Add(product);
                    }

                    await dbContext.SaveChangesAsync();
                }

            }

            if (dbContext.DeliveryMethods.Count() == 0)
            {
                var DeliveryData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/delivery.json");
                var Delivery = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryData);

                if (Delivery?.Count() > 0)
                {
                    foreach (var delivery in Delivery)
                    {
                        dbContext.Set<DeliveryMethod>().Add(delivery);
                    }

                    await dbContext.SaveChangesAsync();
                }

            }

        }
    }
}
