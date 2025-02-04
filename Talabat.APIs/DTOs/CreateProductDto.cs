using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public IFormFile PictureUrl { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int ProductBrandId { get; set; }
        [Required]
        public string Brand { get; set; }

        [Required]
        public int ProductCategoryId { get; set; }
        [Required]
        public string Category { get; set; }
    }
}
