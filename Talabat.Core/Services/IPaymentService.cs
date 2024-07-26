using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Services
{
    public interface IPaymentService
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntentAsync(string basketId);
        Task<Order?> UpdateOrderStatusAsync(string paymentIntentId, bool isPaid);
    }
}
