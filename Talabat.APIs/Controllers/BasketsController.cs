using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
    
    public class BasketsController : APIBaseController
    {
        private readonly IBasketRepository _basketRepository;

        public BasketsController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }


        [HttpGet("{BasketId}")]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string? BasketId)
        {
            var Basket = await _basketRepository.GetBasketAsync(BasketId);
            if(Basket is null )
            {
                return new CustomerBasket(BasketId);
            }
            return Ok(Basket);
        }


        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateOrCreate(CustomerBasket Basket)
        {
            var UpdatedBasket =await _basketRepository.UpdateBasketAsync(Basket);
            if (UpdatedBasket is null)
            {
                return  BadRequest(new ApiResponse(400));
            }
            return Ok(UpdatedBasket);
        }
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }
        
    }
}
