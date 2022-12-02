using System.Security.Claims;

using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DTOs;
using QuizApi.Enums;

namespace QuizApi.Repositories
{
    public interface IQuizesRepository : IRepository
    {
        Task<QuizDTO?> Find(int id);

        IAsyncEnumerable<QuizDTO> Get(
            int pageId,
            int limit,
            string? namePattern,
            int? categoryId,
            int? creatorId,
            CreatorFilter? creatorFilter,
            ClaimsPrincipal user);

        Task<bool> IsNameConflict(string name);

        EntityEntry<QuizDTO> Add(QuizDTO quiz);

        EntityEntry<QuizDTO> Remove(QuizDTO quiz);
    }
}
