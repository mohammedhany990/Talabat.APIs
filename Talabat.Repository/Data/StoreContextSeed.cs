using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Repository.Contexts;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task DataSeed(StoreContext dbstoreContext)
        {
            if(!dbstoreContext.ProductBrands.Any())// to Add them at once
            {
                // As a string, so you will need to convert them again
                var BrandData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                // converting them
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandData);
                if(Brands?.Count() > 0 ) 
                {
                    foreach( var Brand in Brands ) 
                    {
                        await dbstoreContext.Set<ProductBrand>().AddAsync(Brand);
                    }
                    await dbstoreContext.SaveChangesAsync();
                }

            }

            if (!dbstoreContext.ProductTypes.Any())
            {
                var TypesData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/types.json");
                var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);
                if (Types?.Count() > 0)
                {
                    foreach (var Type in Types)
                    {
                        await dbstoreContext.Set<ProductType>().AddAsync(Type);
                    }
                    await dbstoreContext.SaveChangesAsync();
                }

            }

            if (!dbstoreContext.Products.Any())
            {
                var ProductData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductData);
                if (Products?.Count() > 0)
                {
                    foreach (var product in Products)
                    {
                        await dbstoreContext.Set<Product>().AddAsync(product);
                    }
                    await dbstoreContext.SaveChangesAsync();
                }

            }

        }
    }
}
