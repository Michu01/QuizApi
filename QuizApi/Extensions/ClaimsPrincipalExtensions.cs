using System.Security.Claims;

using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Repositories;

namespace QuizApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public static int? TryGetId(this ClaimsPrincipal claimsPrincipal)
        {
            return int.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : null;
        }

        public static Role? TryGetRole(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.FindFirstValue(ClaimTypes.Role) is string value &&
                Enum.TryParse(value, out Role role))
            {
                return role;
            }

            return null;
        }

        public static async Task<bool> CanAccess(this ClaimsPrincipal claimsPrincipal, QuizDTO quiz, IFriendshipsRepository friendshipsRepository)
        {
            if (quiz.Access == Access.Public)
            {
                return true;
            }

            Role? role = claimsPrincipal.TryGetRole();

            if (role == Role.Admin)
            {
                return true;
            }

            int? userId = claimsPrincipal.TryGetId();

            if (userId == quiz.CreatorId)
            {
                return true;
            }

            if (userId.HasValue && quiz.Access == Access.Friends)
            {
                return await friendshipsRepository.AreUsersFriends(userId.Value, quiz.CreatorId);
            }

            return false;
        }

        public static bool CanModify(this ClaimsPrincipal claimsPrincipal, QuizDTO quiz)
        {
            return claimsPrincipal.TryGetRole() == Role.Admin || claimsPrincipal.TryGetId() == quiz.CreatorId;
        }
    }
}
