using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Extensions;
using QuizApi.Models;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly QuizDbContext dbContext;

        private readonly IAuthService authService;

        public UsersController(QuizDbContext dbContext, IAuthService authService)
        {
            this.dbContext = dbContext;
            this.authService = authService;
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

        [HttpGet("Me/QuestionSets")]
        [Authorize]
        public IActionResult GetMyQuestionSets()
        {
            int id = User.GetId();

            IQueryable<QuestionSetDTO> questionSets = dbContext.QuestionSets
                .Where(s => s.CreatorId == id);

            return Ok(questionSets);
        }

        [HttpPost("Me/ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(PasswordChange passwordChange)
        {
            if (passwordChange.CurrentPassword == passwordChange.NewPassword)
            {
                return BadRequest("New password cannot be the same as the old one");
            }

            int id = User.GetId();

            Token token = await authService.ChangePassword(id, passwordChange);

            return Ok(token);
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
