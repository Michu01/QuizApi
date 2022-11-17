using System.ComponentModel.DataAnnotations;

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
                .Where(r => r.ReceiverId == id || r.SenderId == id);

            return Ok(requests);
        }

        [HttpGet("Received")]
        [Authorize]
        public IActionResult GetReceived()
        {
            int id = User.GetId();

            IQueryable<FriendshipRequestDTO> received = dbContext.FriendshipRequests
                .Where(r => r.ReceiverId == id);

            return Ok(received);
        }

        [HttpGet("Sent")]
        [Authorize]
        public IActionResult GetSent()
        {
            int id = User.GetId();

            IQueryable<FriendshipRequestDTO> sent = dbContext.FriendshipRequests
                .Where(r => r.SenderId == id);

            return Ok(sent);
        }

        [HttpPost("Send")]
        [Authorize]
        public async Task<IActionResult> Send([Required] int userId)
        {
            int id = User.GetId();

            if (await dbContext.Users.FindAsync(userId) is not UserDTO receiver)
            {
                return NotFound();
            }

            if (await dbContext.Friendships.FindAsync(userId, id) is not null || 
                await dbContext.Friendships.FindAsync(id, userId) is not null)
            {
                return BadRequest("Already friends");
            }

            FriendshipRequestDTO friendshipRequest = new()
            {
                SenderId = id,
                ReceiverId = userId
            };

            dbContext.FriendshipRequests.Add(friendshipRequest);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Send), friendshipRequest);
        }

        [HttpPost("{senderId:int}/Accept")]
        [Authorize]
        public async Task<IActionResult> Accept(int senderId)
        {
            int myId = User.GetId();

            if (await dbContext.FriendshipRequests.FindAsync(senderId, myId) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            FriendshipDTO friendship = new()
            {
                MeId = myId,
                TheyId = senderId
            };

            dbContext.FriendshipRequests.Remove(friendshipRequest);
            dbContext.Friendships.Add(friendship);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Accept), friendship);
        }

        [HttpDelete("{senderId:int}/Decline")]
        [Authorize]
        public async Task<IActionResult> Decline(int senderId)
        {
            int myId = User.GetId();

            if (await dbContext.FriendshipRequests.FindAsync(senderId, myId) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            dbContext.FriendshipRequests.Remove(friendshipRequest);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
