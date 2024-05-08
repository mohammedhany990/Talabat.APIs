using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks.Dataflow;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
namespace Talabat.APIs.Controllers
{

    public class ProductsController : APIBaseController
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<ProductType> _typeRepository;
        private readonly IGenericRepository<ProductBrand> _brandRepository;

        public ProductsController(IGenericRepository<Product> productRepository,
                                  IMapper mapper,
                                  IGenericRepository<ProductType> TypeRepository,
                                  IGenericRepository<ProductBrand> BrandRepository)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _typeRepository = TypeRepository;
            _brandRepository = BrandRepository;
        }



        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductReturningDTO>>> GetAllProducts([FromQuery]ProductSpecParams Params)
        {
            var Spec = new ProductWithTpyeAndBrandSpecifications(Params);
            var Products = await _productRepository.GetAllWithSpecAsync(Spec);

            var MappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductReturningDTO>>(Products);
            
            var CountSpec = new ProductSpecCount(Params);
            var Count = await _productRepository.GetCountSpecAsync(CountSpec);
            
            return Ok(new Pagination<ProductReturningDTO>(Params.PageSize, Params.PageIndex, MappedProducts, Count));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductReturningDTO),200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var Spec = new ProductWithTpyeAndBrandSpecifications(id);
            var Product = await _productRepository.GetByIdWithSpecAsync(Spec);
            if(Product is null)
            {
                return NotFound(new ApiResponse(404));
            }
            var MappedProduct = _mapper.Map<Product, ProductReturningDTO>(Product);
            return Ok(MappedProduct);
        }


        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetAllTypes()
        {
            var Types = await _typeRepository.GetAllAsync();
            return Ok(Types);
        }


        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetAllBrands()
        {
            var Brands = await _brandRepository.GetAllAsync();
            return Ok(Brands);
        }


    }
}
