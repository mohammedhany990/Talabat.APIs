using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helper;
using Talabat.Core;
using Talabat.Core.Entities.Product;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IMapper mapper,
                                 IUnitOfWork unitOfWork
                                 )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        //[Cache(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductReturningDto>>> GetAllProducts([FromQuery] ProductSpecificationParameters parameters)
        {
            var spec = new ProductWithBrandTypeSpec(parameters);
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var MappedProducts = _mapper.Map<IEnumerable<Product>,
                                                             IEnumerable<ProductReturningDto>>(products);
            var CountSpec = new CountSpec(parameters);

            var count = await _unitOfWork.Repository<Product>().GetSpecCount(CountSpec);

            return Ok(new Pagination<ProductReturningDto>(parameters.PageIndex, parameters.PageSize, count, MappedProducts));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReturningDto>> GetProductById(int id)
        {

            var spec = new ProductWithBrandTypeSpec(id);
            var product = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);
            var MappedProduct = _mapper.Map<Product, ProductReturningDto>(product);

            return product is null ? NotFound(new ApiResponse(404)) : Ok(MappedProduct);
        }



        //[Cache(600)]
        [HttpGet("Brands")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> Brands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }

        //[Cache(600)]
        [HttpGet("Types")]
        public async Task<ActionResult<IEnumerable<ProductType>>> ProductTypes()
        {
            var categories = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(categories);
        }
    }
}
