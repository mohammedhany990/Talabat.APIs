
namespace Talabat.APIs.DTOs
{
    public class UpdateProductDto
    {

        public string? Name { get; set; } 
        public string? Description { get; set; }
        public IFormFile? PictureUrl { get; set; }
        public decimal? Price { get; set; }

        public int? ProductBrandId { get; set; }
        public string? Brand { get; set; }


        public int? ProductCategoryId { get; set; }
        public string? Category { get; set; }
    }
}
