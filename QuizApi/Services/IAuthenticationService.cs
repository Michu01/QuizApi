using QuizApi.Models;

namespace QuizApi.Services
{
    public interface IAuthenticationService
    {
        Task<Token> ChangePassword(int userId, PasswordChange passwordChange);

        Task<Token> ChangeUsername(int userId, UsernameChange usernameChange);

        Task<Token> SignIn(AuthData authData);

        Task<Token> SignUp(AuthData authData);
    }
}