using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Product;

namespace Talabat.APIs.Controllers
{
   
    public class CategoriesController : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //[Cache(600)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> ProductCategories()
        {
            var categories = await _unitOfWork.Repository<ProductCategory>().GetAllAsync();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> Category(int id)
        {
            var category = await _unitOfWork.Repository<ProductCategory>().GetAsync(id);
            if (category is null)
            {
                return NotFound(new ApiResponse(404));
            }
            return Ok(category);
        }
        [HttpPost("add-category")]
        public async Task<ActionResult<ProductCategory>> AddCategory(string categoryName)
        {
            var category = new ProductCategory
            {
                Name = categoryName
            };
            await _unitOfWork.Repository<ProductCategory>().AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return Ok(category);
        }
        [HttpPut("update-category")]
        public async Task<ActionResult<ProductCategory>> UpdateCategory(int id,string categoryName)
        {
            var oldCategory = await _unitOfWork.Repository<ProductCategory>().GetAsync(id);
            if (oldCategory is null)
            {
                return NotFound(new ApiResponse(404));
            }
            oldCategory.Name = categoryName;
             _unitOfWork.Repository<ProductCategory>().Update(oldCategory);
            await _unitOfWork.CompleteAsync();
            return Ok(oldCategory);
        }

        [HttpDelete("delete-category")]
        public async Task<ActionResult> Delete(int id)
        {
            var oldCategory = await _unitOfWork.Repository<ProductCategory>().GetAsync(id);
            if (oldCategory is null)
            {
                return NotFound(new ApiResponse(404));
            }

            _unitOfWork.Repository<ProductCategory>().Delete(oldCategory);
            await _unitOfWork.CompleteAsync();
            return Ok(new ApiResponse(200, "Deleted Successfully!."));
        }



    }
}
