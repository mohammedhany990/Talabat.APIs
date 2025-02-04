using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Product;
using Talabat.Core;

namespace Talabat.APIs.Controllers
{
    
    public class BrandsController : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;


        public BrandsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //[Cache(600)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> ProductBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductBrand>> Category(int id)
        {
            var brand = await _unitOfWork.Repository<ProductBrand>().GetAsync(id);
            if (brand is null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(brand);
        }
        [HttpPost("add-brand")]
        public async Task<ActionResult<ProductBrand>> AddBrand(string brandName)
        {
            var brand = new ProductBrand
            {
                Name = brandName
            };
            await _unitOfWork.Repository<ProductBrand>().AddAsync(brand);
            await _unitOfWork.CompleteAsync();
            return Ok(brand);
        }
        [HttpPut("update-brand")]
        public async Task<ActionResult<ProductBrand>> UpdateCategory(int id, string brandName)
        {
            var oldBrand = await _unitOfWork.Repository<ProductBrand>().GetAsync(id);
            if (oldBrand is null)
            {
                return NotFound(new ApiResponse(404));
            }
            oldBrand.Name = brandName;
            _unitOfWork.Repository<ProductBrand>().Update(oldBrand);
            await _unitOfWork.CompleteAsync();
            return Ok(oldBrand);
        }

        [HttpDelete("delete-brand")]
        public async Task<ActionResult> Delete(int id)
        {
            var oldBrand = await _unitOfWork.Repository<ProductBrand>().GetAsync(id);
            if (oldBrand is null)
            {
                return NotFound(new ApiResponse(404));
            }

            _unitOfWork.Repository<ProductBrand>().Delete(oldBrand);
            await _unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Deleted Successfully!."));
        }
    }
}
