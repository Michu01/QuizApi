using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace QuizApi.Controllers
{
    [Route("Errors")]
    [ApiController]
    public class ErrorsController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            string? message = exception?.InnerException?.Message ?? exception?.Message;

            return BadRequest(message);
        }
    }
}
