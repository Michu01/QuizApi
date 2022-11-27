using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [AllowAnonymous]
        public IActionResult Get(
           int pageId = 0,
           int limit = 10,
           string? namePattern = null,
           bool friendsOnly = false)
        {
            IQueryable<UserDTO> usersQuery = dbContext.Users;

            if (!string.IsNullOrEmpty(namePattern))
            {
                usersQuery = usersQuery.Where(u => u.Name.Contains(namePattern));
            }

            IAsyncEnumerable<UserDTO> users = usersQuery.ToArray().ToAsyncEnumerable();

            if (friendsOnly)
            {
                if (User.Identity == null || !User.Claims.Any())
                {
                    return BadRequest("You must be signed in to view friends");
                }

                int id = User.GetId();

                users = users.WhereAwait(async u => await dbContext.AreUsersFriends(id, u.Id));
            }
 
            users = users.Skip(pageId * limit).Take(limit);

            return Ok(users);
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            int id = User.GetId();

            UserDTO user = (await dbContext.Users.FindAsync(id))!;

            return Ok(user);
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

        [HttpGet("{id:int}/IsFriend")]
        [Authorize]
        public async Task<IActionResult> IsFriend(int id)
        {
            int myId = User.GetId();

            return Ok(await dbContext.AreUsersFriends(myId, id));
        }

        [HttpGet("{id:int}/IsInvited")]
        [Authorize]
        public async Task<IActionResult> IsInvited(int id)
        {
            int myId = User.GetId();

            return Ok(await dbContext.FriendshipRequests.FindAsync(myId, id) is not null);
        }
    }
}
