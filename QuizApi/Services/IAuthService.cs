using QuizApi.Models;

namespace QuizApi.Services
{
    public interface IAuthService
    {
        Task<Token> SignIn(UserAuthData authData);

        Task<Token> SignUp(UserAuthData authData);
    }
}