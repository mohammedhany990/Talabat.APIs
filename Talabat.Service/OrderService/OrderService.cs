using Talabat.Core;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Entities.Product;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Specifications;
using Order = Talabat.Core.Entities.Order_Aggregate.Order;

namespace Talabat.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;


        public OrderService(IBasketRepository basketRepository,
                            IUnitOfWork unitOfWork,
                            IPaymentService paymentService
                            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }


        #region CreateOrderAsync
        public async Task<Order?> CreateOrderAsync(string buyerEmail,
                                                   string basketId,
                                                   int deliveryMethodId,
                                                   UserOrderAddress shippingAddress
                                                   )
        {
            // 1.Get Basket
            var basket = await _basketRepository.GetBasketAsync(basketId);

            // 2.Build Order Items
            var orderItems = new List<OrderItem>();

            if (basket?.Items?.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                    var orderItem = new OrderItem(productItemOrdered, item.Quantity, product.Price);

                    orderItems.Add(orderItem);
                }
            }

            // 3. Calculate Subtotal
            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 4.Get Delivery Method
            var deliverymethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);

            // 5. Create Order
            var spec = new OrderWithPaymentIntentSpec(basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
            }

            var order = new Order(
                buyerEmail: buyerEmail,
                shippingAddress: shippingAddress,
                deliveryMethod: deliverymethod,
                items: orderItems,
                subTotal: subtotal,
                paymentIntent: basket.PaymentIntentId
                );

            // 6. Add Order to database
            await _unitOfWork.Repository<Order>().AddAsync(order);

            // 7. Save Changes
            var result = await _unitOfWork.CompleteAsync();


            return result <= 0 ? null : order;
        }
        #endregion

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpec(buyerEmail);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return orders;
        }

        public async Task<Order> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
        {
            var spec = new OrderSpec(buyerEmail, orderId);
            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            var deliveries = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return deliveries;
        }
    }
}
