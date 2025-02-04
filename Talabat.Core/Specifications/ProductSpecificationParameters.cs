namespace Talabat.Core.Specifications
{
    public class ProductSpecificationParameters
    {
        public string? sort { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }

        private int pageSize = 5;

        public string? search { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > 10 ? 10 : value;
        }
        public int PageIndex { get; set; } = 1;




    }
}
