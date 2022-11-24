using QuizApi.Models;

namespace QuizApi.Services
{
    public interface IAuthService
    {
        Task<Token> ChangePassword(int id, PasswordChange passwordChange);

        Task<Token> SignIn(AuthData authData);

        Task<Token> SignUp(AuthData authData);
    }
}