using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Talabat.Core.Entities.Product;

namespace Talabat.Repository.Config.ProductConfigurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(P => P.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(P => P.Description)
                   .IsRequired();

            builder.Property(P => P.PictureUrl)
                   .IsRequired();

            builder.Property(P => P.Price)
                   .HasColumnType("dec(18,2)");

            builder.HasOne(P => P.ProductBrand)
                   .WithMany()
                   .HasForeignKey(FK => FK.ProductBrandId);

            builder.HasOne(P => P.ProductType)
                   .WithMany()
                   .HasForeignKey(FK => FK.ProductTypeId); ;



        }
    }
}
