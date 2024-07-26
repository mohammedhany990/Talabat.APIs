namespace Talabat.APIs.DTOs
{
    public class ProductReturningDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public decimal Price { get; set; }

        public int ProductBrandId { get; set; }
        public string Brand { get; set; }


        public int ProductTypeId { get; set; }
        public string Category { get; set; }
    }
}
