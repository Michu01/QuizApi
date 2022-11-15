using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuizApi.DTOs;
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
        public async Task<IActionResult> SignIn([FromBody] UserAuthData authData)
        {
            try
            {
                (UserDTO user, string token) = await authService.SignIn(authData);

                return Ok(new { user, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] UserAuthData authData)
        {
            try
            {
                (UserDTO user, string token) = await authService.SignUp(authData);

                return Ok(new { user, token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
