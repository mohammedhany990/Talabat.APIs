namespace Talabat.Core.Entities.Order_Aggregate
{
    public class ProductItemOrdered
    {
        private ProductItemOrdered()
        {

        }
        public ProductItemOrdered(int productId, string productName, string pictureUrl)
        {
            ProductId = productId;
            PictureUrl = pictureUrl;
            ProductName = productName;
        }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
    }
}
