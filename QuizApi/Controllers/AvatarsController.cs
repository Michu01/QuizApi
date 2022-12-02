using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.Extensions;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarsController : ControllerBase
    {
        private readonly IAvatarService avatarService;

        public AvatarsController(IAvatarService avatarService)
        {
            this.avatarService = avatarService;
        }

        [HttpGet("Path")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> GetPath()
        {
            int id = User.GetId();

            string? result = avatarService.GetPath(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{id:int}/Path")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> GetPath(int id)
        {
            string? result = avatarService.GetPath(id);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("Change")]
        [Authorize]
        [RequestSizeLimit(2 << 20)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Change([Required] IFormFile file)
        {
            int id = User.GetId();

            string path = await avatarService.Change(file, id);

            return Created(path, null);
        }
    }
}