namespace Talabat.Core.Entities.Product
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int ProductBrandId { get; set; }
        public ProductBrand ProductBrand { get; set; }


        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

    }
}
