using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Contexts
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options):base(options) 
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Apply All Configuraions classes that implement IEntityTypeConfigurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

    }
}
