using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DTOs;
using QuizApi.Extensions;
using QuizApi.Repositories;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private const int MaxUserLimit = 100;

        private readonly IUsersRepository repository;

        private readonly IFriendshipsRepository friendshipsRepository;

        private readonly IFriendshipRequestsRepository friendshipRequestsRepository;

        public UsersController(
            IUsersRepository repository, 
            IFriendshipsRepository friendshipsRepository, 
            IFriendshipRequestsRepository friendshipRequestsRepository)
        {
            this.repository = repository;
            this.friendshipsRepository = friendshipsRepository;
            this.friendshipRequestsRepository = friendshipRequestsRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IAsyncEnumerable<UserDTO>> Get(
           int pageId = 0,
           int limit = 10,
           string? namePattern = null,
           bool friendsOnly = false)
        {
            limit = Math.Min(limit, MaxUserLimit);

            if (friendsOnly && User.Identity is null)
            {
                return Unauthorized();
            }

            int? userId = User.TryGetId();

            IAsyncEnumerable<UserDTO> users = repository.Get(pageId, limit, namePattern, friendsOnly, userId);

            return Ok(users);
        }

        [HttpGet("Me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDTO>> Get()
        {
            int id = User.GetId();

            UserDTO user = (await repository.Find(id))!;

            return Ok(user);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            if (await repository.Find(id) is not UserDTO user)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet("{id:int}/IsFriend")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> IsFriend(int id)
        {
            int myId = User.GetId();

            bool isFriend = await friendshipsRepository.AreUsersFriends(myId, id);

            return Ok(isFriend);
        }

        [HttpGet("{id:int}/IsInvited")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> IsInvited(int id)
        {
            int myId = User.GetId();

            bool isInvited = await friendshipRequestsRepository.IsInvited(myId, id);

            return Ok(isInvited);
        }
    }
}