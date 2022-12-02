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
            _ = int.TryParse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out int id);
            return id;
        }

        public static Role? GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Role) is string value ? Enum.Parse<Role>(value) : null;
        }

        public static async Task<bool> CanAccess(this ClaimsPrincipal claimsPrincipal, QuizDTO quiz, IFriendshipsRepository friendshipsRepository)
        {
            if (quiz.Access == Access.Public)
            {
                return true;
            }

            if (claimsPrincipal.Identity is null || claimsPrincipal.GetRole() is not Role role)
            {
                return false;
            }

            if (role == Role.Admin)
            {
                return true;
            }

            int userId = claimsPrincipal.GetId();

            if (userId == quiz.CreatorId)
            {
                return true;
            }

            if (quiz.Access == Access.Friends)
            {
                return await friendshipsRepository.AreUsersFriends(userId, quiz.CreatorId);
            }

            return false;
        }

        public static bool CanModify(this ClaimsPrincipal claimsPrincipal, QuizDTO quiz)
        {
            return claimsPrincipal.GetRole() == Role.Admin || claimsPrincipal.GetId() == quiz.CreatorId;
        }
    }
}
