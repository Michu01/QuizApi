using System.Security.Claims;

using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Services
{
    public interface IAuthService
    {
        Task<(UserDTO, string)> SignIn(UserAuthData authData);

        Task<(UserDTO, string)> SignUp(UserAuthData authData);
    }
}