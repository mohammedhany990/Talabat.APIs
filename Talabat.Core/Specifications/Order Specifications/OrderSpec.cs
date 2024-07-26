using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Specifications
{
    public class OrderSpec : BaseSpecifications<Order>
    {
        public OrderSpec(string email)
            : base(O => O.BuyerEmail == email)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
            ApplyOrderByAsc(O => O.OrderDate);
        }

        public OrderSpec(string buyerEmail, int orderId) :
            base(O => O.BuyerEmail == buyerEmail && O.Id == orderId)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
        }
    }
}
