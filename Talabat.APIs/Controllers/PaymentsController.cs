using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsController> _logger;
        private const string EndpointSecret = "whsec_ca723e1e0ad297b7c5ab134022aafeb90e12a26c6321e2f191749b4974a15741";

        public PaymentsController(IPaymentService paymentService, IMapper mapper, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePayment(string basketId)
        {
            var customerBasket = await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId);
            if (customerBasket is null)
            {
                return BadRequest(new ApiResponse(400, "There's a problem with your Basket."));
            }

            var MappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(customerBasket);

            return Ok(MappedBasket);
        }

        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], EndpointSecret, 300, false);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
            // Handle the event
            Order? order;
            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                    order = await _paymentService.UpdateOrderStatusAsync(paymentIntent.Id, true);
                    _logger.LogInformation("Order is succeeded {0}", paymentIntent?.Id);
                    _logger.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
                    break;
                case Events.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdateOrderStatusAsync(paymentIntent.Id, false);
                    _logger.LogInformation("Order is failed {0}", paymentIntent?.Id);
                    _logger.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }

            Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);

            return Ok();
        }

    }
}
