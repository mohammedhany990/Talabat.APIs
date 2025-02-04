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

        [HttpPost("add-product")]
        public async Task<ActionResult<ProductReturningDto>> AddProduct([FromForm] CreateProductDto productDto)
        {
            var category = await  _unitOfWork.Repository<ProductCategory>()
                .GetAsync(productDto.ProductCategoryId);

            var brand = await _unitOfWork.Repository<ProductBrand>()
                .GetAsync(productDto.ProductBrandId);


            var product = _mapper.Map<CreateProductDto, Product>(productDto);
            product.ProductCategory = category;
            product.ProductBrand = brand;

            product.PictureUrl = FileSettings.UploadFile(productDto.PictureUrl, "products");

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.CompleteAsync();

            var MappedProduct = _mapper.Map<Product, ProductReturningDto>(product);

            return product is null ? NotFound(new ApiResponse(404)) : Ok(MappedProduct);
        }
        /*
        [HttpPut("update-product")]
        public async Task<ActionResult<ProductReturningDto>> UpdateProduct(int id,[FromForm] CreateProductDto productDto)
        {
            var spec = new ProductWithBrandTypeSpec(id);
            var oldProduct = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            if (oldProduct is null)
            {
                return NotFound(new ApiResponse(404));
            }

            var category = await _unitOfWork.Repository<ProductCategory>()
                .GetAsync(productDto.ProductCategoryId);

            var brand = await _unitOfWork.Repository<ProductBrand>()
                .GetAsync(productDto.ProductBrandId);
            _mapper.Map(productDto, oldProduct);


            oldProduct.ProductCategory = category;
            oldProduct.ProductBrand = brand;

            if (productDto.PictureUrl is not null)
            {
                var imageName = oldProduct.PictureUrl.Split('/').Last();

                FileSettings.DeleteFile(imageName, "products");

                oldProduct.PictureUrl = FileSettings.UploadFile(productDto.PictureUrl, "products");

            }


            _unitOfWork.Repository<Product>().Update(oldProduct);
            await _unitOfWork.CompleteAsync();

            var mappedProduct = _mapper.Map<Product, ProductReturningDto>(oldProduct);

            return mappedProduct is null ? NotFound(new ApiResponse(404)) : Ok(mappedProduct);
        }*/
     
        [HttpPut("update-product")]
        public async Task<ActionResult<ProductReturningDto>> UpdateProduct(int id, [FromForm] UpdateProductDto? productDto)
        {
            var spec = new ProductWithBrandTypeSpec(id);
            var oldProduct = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            if (oldProduct is null)
            {
                return NotFound(new ApiResponse(404));
            }

            if (!string.IsNullOrEmpty(productDto.Name))
            {
                oldProduct.Name = productDto.Name;
            }

            if (!string.IsNullOrEmpty(productDto.Description))
            {
                oldProduct.Description = productDto.Description;
            }

            if (productDto.Price.HasValue)
            {
                oldProduct.Price = productDto.Price.Value;
            }

            if (productDto.ProductCategoryId != oldProduct.ProductCategoryId && productDto.ProductCategoryId is not null)
            {
                var category = await _unitOfWork.Repository<ProductCategory>()
                    .GetAsync(productDto.ProductCategoryId.Value);

                if (category is null)
                {
                    return BadRequest(new ApiResponse(400, "Invalid category ID"));
                }

                oldProduct.ProductCategory = category;
            }

            if (productDto.ProductBrandId != oldProduct.ProductBrandId && productDto.ProductBrandId is not null)
            {
                var brand = await _unitOfWork.Repository<ProductBrand>()
                    .GetAsync(productDto.ProductBrandId.Value);

                if (brand is null)
                {
                    return BadRequest(new ApiResponse(400, "Invalid brand ID"));
                }

                oldProduct.ProductBrand = brand;
            }

            if (productDto.PictureUrl is not null)
            {
                var imageName = oldProduct.PictureUrl?.Split('/').Last();

                if (!string.IsNullOrEmpty(imageName))
                {
                    FileSettings.DeleteFile(imageName, "products");
                }

                oldProduct.PictureUrl = FileSettings.UploadFile(productDto.PictureUrl, "products");
            }

            await _unitOfWork.CompleteAsync();

            var mappedProduct = _mapper.Map<Product, ProductReturningDto>(oldProduct);

            return Ok(mappedProduct);
        }


        [HttpDelete("delete-product")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetAsync(id);
            if (product is null)
            {
                return NotFound(new ApiResponse(404));
            }

            var imageName = product.PictureUrl.Split('/').Last();
            
            FileSettings.DeleteFile(imageName, "products");

            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Deleted!"));

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

       
    }
}
