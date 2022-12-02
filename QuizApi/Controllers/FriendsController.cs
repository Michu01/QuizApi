using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DTOs;

using QuizApi.Extensions;
using QuizApi.Repositories;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendshipsRepository repository;

        private readonly IUsersRepository usersRepository;

        public FriendsController(IFriendshipsRepository repository, IUsersRepository usersRepository)
        {
            this.repository = repository;
            this.usersRepository = usersRepository;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
            int id = User.GetId();

            IEnumerable<UserDTO> friends = await usersRepository.GetFriends(id);

            return Ok(friends);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            int myId = User.GetId();

            if ((await repository.Find(myId, id) ?? await repository.Find(id, myId)) is not FriendshipDTO friendship)
            {
                return NotFound();
            }

            repository.Remove(friendship);
            await repository.SaveChangesAsync();

            return Ok();
        }
    }
}
