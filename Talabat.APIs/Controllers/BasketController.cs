using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
    public class BasketController : ApiBaseController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }



        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string? id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            return Ok(basket is null ? new CustomerBasket(id) : basket);
        }



        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasketDto Basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(Basket);
            var UpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

            return Basket is null ? BadRequest(new ApiResponse(400)) : Ok(UpdatedBasket);
        }



        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            return await _basketRepository.DeleteBasketAsync(id);
        }

    }
}
