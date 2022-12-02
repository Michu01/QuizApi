using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;

using QuizApi.Extensions;
using QuizApi.Repositories;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipRequestsController : ControllerBase
    {
        private readonly IFriendshipRequestsRepository repository;

        private readonly IFriendshipsRepository friendshipsRepository;

        private readonly IUsersRepository usersRepository;

        public FriendshipRequestsController(
            IFriendshipRequestsRepository repository, 
            IFriendshipsRepository friendshipsRepository,
            IUsersRepository usersRepository)
        {
            this.repository = repository;
            this.friendshipsRepository = friendshipsRepository;
            this.usersRepository = usersRepository;
        }

        [HttpGet("{receiverId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FriendshipRequestDTO>> Get(int receiverId)
        {
            int id = User.GetId();

            if (await repository.Find(id, receiverId) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            return Ok(friendshipRequest);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<FriendshipRequestDTO>> Get()
        {
            int id = User.GetId();

            IEnumerable<FriendshipRequestDTO> requests = repository.Get(id);

            return Ok(requests);
        }

        [HttpGet("Received")]
        [Authorize]
        public ActionResult<IEnumerable<FriendshipRequestDTO>> GetReceived()
        {
            int id = User.GetId();

            IEnumerable<FriendshipRequestDTO> received = repository.GetReceived(id);

            return Ok(received);
        }

        [HttpGet("Sent")]
        [Authorize]
        public ActionResult<IEnumerable<FriendshipRequestDTO>> GetSent()
        {
            int id = User.GetId();

            IEnumerable<FriendshipRequestDTO> sent = repository.GetSent(id);

            return Ok(sent);
        }

        [HttpPost("Send/{receiverId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FriendshipRequestDTO>> Send(int receiverId)
        {
            int id = User.GetId();

            if (id == receiverId)
            {
                return BadRequest("You cannot be friends with yourself! Find some real friends...");
            }

            if (await usersRepository.Find(receiverId) is not UserDTO receiver)
            {
                return NotFound();
            }

            if (await friendshipsRepository.AreUsersFriends(id, receiverId))
            {
                return BadRequest("Already friends");
            }

            FriendshipRequestDTO friendshipRequest = new()
            {
                SenderId = id,
                ReceiverId = receiverId
            };

            repository.Add(friendshipRequest);
            await repository.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { receiverId }, friendshipRequest);
        }

        [HttpDelete("Cancel/{userId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int userId)
        {
            int id = User.GetId();

            if (await repository.Find(id, userId) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            repository.Remove(friendshipRequest);
            await repository.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Accept/{senderId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FriendshipDTO>> Accept(int senderId)
        {
            int myId = User.GetId();

            if (await repository.Find(senderId, myId) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            FriendshipDTO friendship = new()
            {
                FirstUserId = myId,
                SecondUserId = senderId
            };

            repository.Remove(friendshipRequest);
            friendshipsRepository.Add(friendship);
            await repository.SaveChangesAsync();

            return CreatedAtAction(nameof(FriendsController.Get), new { id = senderId }, friendship);
        }

        [HttpDelete("Decline/{senderId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Decline(int senderId)
        {
            int myId = User.GetId();

            if (await repository.Find(senderId, myId) is not FriendshipRequestDTO friendshipRequest)
            {
                return NotFound();
            }

            repository.Remove(friendshipRequest);
            await repository.SaveChangesAsync();

            return Ok();
        }
    }
}
