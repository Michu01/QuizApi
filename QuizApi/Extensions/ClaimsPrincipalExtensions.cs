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

        public static UserRole? GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Role) is string value ? Enum.Parse<UserRole>(value) : null;
        }

        public static async Task<bool> CanAccess(this ClaimsPrincipal claimsPrincipal, QuestionSetDTO questionSet, QuizDbContext dbContext)
        {
            if (questionSet.Access == QuestionSetAccess.Public)
            {
                return true;
            }

            if (claimsPrincipal.Identity is null || claimsPrincipal.GetRole() is not UserRole role)
            {
                return false;
            }

            if (role == UserRole.Admin)
            {
                return true;
            }

            int userId = claimsPrincipal.GetId();

            return questionSet.Access switch
            {
                QuestionSetAccess.Private => userId == questionSet.CreatorId,
                QuestionSetAccess.Friends => await dbContext.AreUsersFriends(userId, questionSet.CreatorId),
                _ => throw new NotImplementedException()
            };
        }

        public static bool CanModify(this ClaimsPrincipal claimsPrincipal, QuestionSetDTO questionSet)
        {
            return claimsPrincipal.GetRole() == UserRole.Admin || 
                claimsPrincipal.GetId() == questionSet.CreatorId;
        }
    }
}
