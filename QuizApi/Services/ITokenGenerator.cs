using QuizApi.DTOs;
using QuizApi.Models;

namespace QuizApi.Services
{
    public interface ITokenGenerator
    {
        Token Generate(UserDTO user);
    }
}
