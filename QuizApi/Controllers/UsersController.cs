using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Extensions;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        private readonly IWebHostEnvironment hostEnvironment;

        public UsersController(QuizDbContext dbContext, IWebHostEnvironment hostEnvironment)
        {
            this.dbContext = dbContext;
            this.hostEnvironment = hostEnvironment;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get(
           int pageId = 0,
           int limit = 10,
           string? namePattern = null)
        {
            IQueryable<UserDTO> users = dbContext.Users;

            if (!string.IsNullOrEmpty(namePattern))
            {
                users = users.Where(u => u.Name.Contains(namePattern));
            }
 
            users = users.Skip(pageId * limit).Take(limit);

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            if (await dbContext.Users.FindAsync(id) is not UserDTO user)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            int id = User.GetId();

            UserDTO user = (await dbContext.Users.FindAsync(id))!;

            return Ok(user);
        }

        [HttpGet("Friends")]
        [Authorize]
        public IActionResult GetFriends()
        {
            int id = User.GetId();

            IQueryable<FriendshipDTO> friends = dbContext.Friendships
                .Where(r => r.MeId == id || r.TheyId == id);

            return Ok(friends);
        }

        [HttpPost("ChangeAvatar")]
        [Authorize]
        [RequestSizeLimit(2 << 20)]
        public async Task<IActionResult> ChangeAvatar([Required] [FileExtensions] IFormFile formFile)
        {
            string? extension = Path.GetExtension(formFile.FileName);

            string path = Path.Combine(hostEnvironment.WebRootPath, "images", "avatars", $"{User.GetId()}{extension}");

            using FileStream file = new(path, FileMode.Create);

            await formFile.CopyToAsync(file);

            return Created(path, null);
        }

        [HttpDelete("Friends/{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteFriend(int id)
        {
            int myId = User.GetId();

            UserDTO me = (await dbContext.Users.FindAsync(myId))!;

            if (me.Friendships.SingleOrDefault(f => f.TheyId == id) is not FriendshipDTO friendship)
            {
                return NotFound();
            }

            dbContext.Friendships.Remove(friendship);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
