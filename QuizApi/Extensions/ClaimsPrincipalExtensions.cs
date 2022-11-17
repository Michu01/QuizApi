using System.Security.Claims;

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

        public static UserRole GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            return Enum.Parse<UserRole>(claimsPrincipal.FindFirstValue(ClaimTypes.Role));
        }

        public static bool CanAccess(this ClaimsPrincipal claimsPrincipal, QuestionSetDTO questionSet)
        {
            if (questionSet.Access == QuestionSetAccess.Public)
            {
                return true;
            }

            if (claimsPrincipal.Identity is null)
            {
                return false;
            }

            if (claimsPrincipal.GetRole() == UserRole.Admin)
            {
                return true;
            }

            int userId = claimsPrincipal.GetId();

            return questionSet.Access switch
            {
                QuestionSetAccess.Private => userId == questionSet.CreatorId,
                QuestionSetAccess.Friends => questionSet.Creator.IsFriend(userId),
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
