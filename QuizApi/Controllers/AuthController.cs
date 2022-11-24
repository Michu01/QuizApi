using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.Extensions;
using QuizApi.Models;
using QuizApi.Services;

namespace QuizApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody][Required] AuthData authData)
        {
            Token token = await authService.SignIn(authData);

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody][Required] AuthData authData)
        {
            Token token = await authService.SignUp(authData);

            return Ok(token);
        }

        [HttpPost("ChangePassword")]
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
    }
}
