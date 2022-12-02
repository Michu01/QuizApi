using System.Security.Claims;

using QuizApi.DTOs;

namespace QuizApi.Services
{
    public interface IAuthorizationService
    {
        Task<bool> CanUserAccessQuiz(ClaimsPrincipal user, QuizDTO quiz);

        bool CanUserModifyQuiz(ClaimsPrincipal user, QuizDTO quiz);
    }
}