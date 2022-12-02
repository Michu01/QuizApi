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
        private readonly IAuthenticationService authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<ActionResult<Token>> SignIn([Required] AuthData authData)
        {
            Token token = await authenticationService.SignIn(authData);

            return Ok(token);
        }

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<ActionResult<Token>> SignUp([Required] AuthData authData)
        {
            Token token = await authenticationService.SignUp(authData);

            return Ok(token);
        }

        [HttpPost("ChangeUsername")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Token>> ChangeUsername([Required] UsernameChange usernameChange)
        {
            int id = User.GetId();

            Token token = await authenticationService.ChangeUsername(id, usernameChange);

            return Ok(token);
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Token>> ChangePassword([Required] PasswordChange passwordChange)
        {
            int id = User.GetId();

            Token token = await authenticationService.ChangePassword(id, passwordChange);

            return Ok(token);
        }
    }
}
