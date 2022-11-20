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

        private readonly ISet<string> allowedExtensions = new HashSet<string>()
        {
            ".jpg", ".png", ".gif", ".jpeg"
        };

        public UserAvatarsController(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        private string AvatarsPath => Path.Combine(hostEnvironment.WebRootPath, "images", "avatars");

        private string[] GetMatches(int id) => Directory.GetFiles(AvatarsPath, $"{id}.*");

        [HttpGet("Path")]
        [Authorize]
        public IActionResult GetPath()
        {
            int id = User.GetId();

            string[] matches = GetMatches(id);

            if (matches.Length == 0)
            {
                return NotFound();
            }

            string result = Path.GetRelativePath(hostEnvironment.WebRootPath, matches[0]);

            return Ok(result);
        }

        [HttpPost("Change")]
        [Authorize]
        [RequestSizeLimit(2 << 20)]
        public async Task<IActionResult> Change([Required] IFormFile file)
        {
            string? extension = Path.GetExtension(file.FileName);

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest($"Invalid extension. Accepted extensions are: {string.Join(',', allowedExtensions)}");
            }

            int id = User.GetId();

            foreach (string match in GetMatches(id))
            {
                System.IO.File.Delete(match);
            }

            string path = Path.Combine(AvatarsPath, $"{User.GetId()}{extension}");

            using FileStream fileStream = new(path, FileMode.Create);

            await file.CopyToAsync(fileStream);

            return Created(path, null);
        }
    }
}
