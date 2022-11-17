using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
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

        [HttpGet("{id:int}/QuestionSets")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestionSets(int id)
        {
            if (await dbContext.Users.FindAsync(id) is null)
            {
                return NotFound();
            }

            IQueryable<QuestionSetDTO> questionSets = dbContext.QuestionSets
                .Where(s => s.CreatorId == id);

            return Ok(questionSets);
        }
    }
}
