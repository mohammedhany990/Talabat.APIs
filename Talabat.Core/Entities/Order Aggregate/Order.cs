
namespace Talabat.Core.Entities.Order_Aggregate
{
    public class Order : BaseEntity
    {

        private Order()
        {

        }
        public Order(string buyerEmail,
                     UserOrderAddress shippingAddress,
                     DeliveryMethod? deliveryMethod,
                     ICollection<OrderItem> items,
                     decimal subTotal,
                     string paymentIntent
                     )
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntent;
        }

        public string BuyerEmail { get; set; } = null!;
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public UserOrderAddress ShippingAddress { get; set; }
        public DeliveryMethod? DeliveryMethod { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
        public decimal SubTotal { get; set; }
        public decimal GetTotal() => SubTotal + DeliveryMethod.Cost;
        public string PaymentIntentId { get; set; }

    }
}

