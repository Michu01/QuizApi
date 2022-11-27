using System.Security.Claims;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;

namespace QuizApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return int.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public static Role? GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Role) is string value ? Enum.Parse<Role>(value) : null;
        }

        public static async Task<bool> CanAccess(this ClaimsPrincipal claimsPrincipal, QuizDTO questionSet, QuizDbContext dbContext)
        {
            if (questionSet.Access == Access.Public)
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

            if (userId == questionSet.CreatorId)
            {
                return true;
            }

            if (questionSet.Access == Access.Friends)
            {
                return await dbContext.AreUsersFriends(userId, questionSet.CreatorId);
            }

            return false;
        }

        public static bool CanModify(this ClaimsPrincipal claimsPrincipal, QuizDTO questionSet)
        {
            return claimsPrincipal.GetRole() == Role.Admin || 
                claimsPrincipal.GetId() == questionSet.CreatorId;
        }
    }
}
