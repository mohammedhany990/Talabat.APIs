using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
    // When trying reach Endpoint not existing, will redirect to this..
    [Route("errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi =true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Error(int code)
        {
            return NotFound(new ApiResponse(code));
        }
    }
}
