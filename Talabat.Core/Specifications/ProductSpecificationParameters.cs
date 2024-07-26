namespace Talabat.Core.Specifications
{
    public class ProductSpecificationParameters
    {
        public string? sort { get; set; }
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }

        private int pagesize = 5;

        public string? search { get; set; }
        public string? TypeName { get; set; }
        public string? BrandName { get; set; }

        public int PageSize
        {
            get { return pagesize; }
            set { pagesize = value > 10 ? 10 : value; }
        }
        public int PageIndex { get; set; } = 1;




    }
}
