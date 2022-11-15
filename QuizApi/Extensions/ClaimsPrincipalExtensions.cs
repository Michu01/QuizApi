using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

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
    }
}
