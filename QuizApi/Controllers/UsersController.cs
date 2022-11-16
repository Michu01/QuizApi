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

        public UsersController(QuizDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
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

        [HttpGet("Friends")]
        [Authorize]
        public async Task<IActionResult> GetFriends()
        {
            int id = User.GetId();

            UserDTO user = (await dbContext.Users.FindAsync(id))!;

            IEnumerable<UserDTO> friends = user.Friendships.Select(f => f.They);

            return Ok(friends);
        }
    }
}
