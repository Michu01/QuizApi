using System.Security.Claims;

using QuizApi.DTOs;
using QuizApi.Extensions;
using QuizApi.Repositories;

namespace QuizApi.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IFriendshipsRepository friendshipsRepository;

        public AuthorizationService(IFriendshipsRepository friendshipsRepository)
        {
            this.friendshipsRepository = friendshipsRepository;
        }

        public async Task<bool> CanUserAccessQuiz(ClaimsPrincipal user, QuizDTO quiz)
        {
            return await user.CanAccess(quiz, friendshipsRepository);
        }

        public bool CanUserModifyQuiz(ClaimsPrincipal user, QuizDTO quiz)
        {
            return user.CanModify(quiz);
        }
    }
}