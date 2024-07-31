using Microsoft.Extensions.Configuration;
using Stripe;
using Talabat.Core;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Specifications;

namespace Talabat.Service.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration,
                              IBasketRepository basketRepository,
                              IUnitOfWork unitOfWork
        )
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }


        #region CreateOrUpdatePaymentIntentAsync
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntentAsync(string basketId)
        {
            // 1. Secret Key
            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            // 2. Basket 
            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket is null)
            {
                return null;
            }

            var shippingPrice = 0M;

            if (basket.DeliveryMethodId.HasValue)
            {
                var delivery = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                shippingPrice = delivery.Cost;
                basket.ShippingPrice = shippingPrice;
            }

            // 3. Total
            if (basket.Items.Count > 0)
            {
                var productRepository = _unitOfWork.Repository<Core.Entities.Product.Product>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepository.GetAsync(item.Id);
                    if (product.Price != item.Price)
                    {
                        item.Price = product.Price;
                    }
                }
            }
            var subTotal = basket.Items.Sum(I => I.Price * I.Quantity);


            // 4. Create Payment Intent
            var Service = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId)) // Create
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(subTotal * 100 + shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await Service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else // Update
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(subTotal * 100 + shippingPrice * 100),
                };
                paymentIntent = await Service.UpdateAsync(basket.PaymentIntentId, options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }
        #endregion



        #region UpdateOrderStatusAsync
        public async Task<Order?> UpdateOrderStatusAsync(string paymentIntentId, bool isPaid)
        {
            var spec = new OrderWithPaymentIntentSpec(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (order is null)
            {
                return null;
            }
            order.Status = isPaid ? OrderStatus.PaymentReceived
                                  : OrderStatus.PaymentFailed;


            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            return order;
        }
        #endregion

    }
}
