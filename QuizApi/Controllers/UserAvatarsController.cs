using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using QuizApi.Extensions;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAvatarsController : ControllerBase
    {
        private readonly IWebHostEnvironment hostEnvironment;

        public UserAvatarsController(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(2 << 20)]
        public async Task<IActionResult> ChangeAvatar([Required][FileExtensions] IFormFile formFile)
        {
            string? extension = Path.GetExtension(formFile.FileName);

            string path = Path.Combine(hostEnvironment.WebRootPath, "images", "avatars", $"{User.GetId()}{extension}");

            using FileStream file = new(path, FileMode.Create);

            await formFile.CopyToAsync(file);

            return Created(path, null);
        }
    }
}
