using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;

using QuizApi.Extensions;
using QuizApi.Models;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        public FriendsController(QuizDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            int id = User.GetId();

            IQueryable<User> friends = dbContext.Friendships
                .Where(f => f.MeId == id || f.TheyId == id)
                .Select(f => new User(f.MeId == id ? f.TheyId : f.MeId));

            return Ok(friends);
        }

       

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            int myId = User.GetId();

            if ((await dbContext.Friendships.FindAsync(id, myId) ?? 
                await dbContext.Friendships.FindAsync(myId, id)) 
                is not FriendshipDTO friendship)
            {
                return NotFound();
            }

            dbContext.Friendships.Remove(friendship);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
