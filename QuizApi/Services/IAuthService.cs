using QuizApi.Models;

namespace QuizApi.Services
{
    public interface IAuthService
    {
        Task<Token> ChangePassword(int id, PasswordChange passwordChange);

        Task<Token> SignIn(UserAuthData authData);

        Task<Token> SignUp(UserAuthData authData);
    }
}