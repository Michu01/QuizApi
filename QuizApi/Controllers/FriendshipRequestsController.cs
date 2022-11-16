using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;

using QuizApi.Extensions;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipRequestsController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        public FriendshipRequestsController(QuizDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            int id = User.GetId();

            IQueryable<FriendshipRequestDTO> requests = dbContext.FriendshipRequests
                .Where(r => r.ReceiverId == id);

            return Ok(requests);
        }

        [HttpPost("Send")]
        [Authorize]
        public async Task<IActionResult> Send(int userId)
        {
            int id = User.GetId();

            UserDTO sender = (await dbContext.Users.FindAsync(id))!;

            if (await dbContext.Users.FindAsync(userId) is not UserDTO receiver)
            {
                return NotFound();
            }

            if (sender.Friendships.Any(f => f.TheyId == userId))
            {
                return BadRequest("Already friends");
            }

            FriendshipRequestDTO friendshipRequest = new()
            {
                SenderId = User.GetId(),
                ReceiverId = id
            };

            dbContext.FriendshipRequests.Add(friendshipRequest);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id:int}/Accept")]
        [Authorize]
        public async Task<IActionResult> Accept(int id)
        {
            if (await dbContext.FriendshipRequests.FindAsync(id) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            if (friendshipRequest.ReceiverId != User.GetId())
            {
                return Forbid();
            }

            FriendshipDTO friendship = new()
            {
                MeId = User.GetId(),
                TheyId = id
            };

            dbContext.FriendshipRequests.Remove(friendshipRequest);
            dbContext.Friendships.Add(friendship);
            await dbContext.SaveChangesAsync();

            return Ok(friendship);
        }

        [HttpPost("{id:int}/Decline")]
        [Authorize]
        public async Task<IActionResult> Decline(int id)
        {
            if (await dbContext.FriendshipRequests.FindAsync(id) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            if (friendshipRequest.ReceiverId != User.GetId())
            {
                return Forbid();
            }

            dbContext.FriendshipRequests.Remove(friendshipRequest);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
