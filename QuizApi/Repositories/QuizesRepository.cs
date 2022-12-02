using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Enums;
using QuizApi.Extensions;
using QuizApi.Services;

namespace QuizApi.Repositories
{
    public class QuizesRepository : Repository, IQuizesRepository
    {
        private readonly IAuthorizationService authorizationService;

        private DbSet<QuizDTO> Quizes => DbContext.Quizes;

        public QuizesRepository(QuizDbContext dbContext, IAuthorizationService authorizationService) : base(dbContext) 
        {
            this.authorizationService = authorizationService;
        }

        private IEnumerable<QuizDTO> GetUserFriendsQuizes(int userId)
        {
            return DbContext.Friendships
                .Where(f => f.FirstUserId == userId || f.SecondUserId == userId)
                .Join(Quizes, f => f.FirstUserId == userId ? f.SecondUserId : f.FirstUserId, qs => qs.CreatorId, (_, qs) => qs);
        }

        public async Task<QuizDTO?> Find(int id)
        {
            return await Quizes.FindAsync(id);
        }

        public IAsyncEnumerable<QuizDTO> Get(
            int pageId, 
            int limit, 
            string? namePattern, 
            int? categoryId, 
            int? creatorId, 
            CreatorFilter? creatorFilter,
            ClaimsPrincipal user)
        {
            IQueryable<QuizDTO> quizesQuery = Quizes;

            if (!string.IsNullOrEmpty(namePattern))
            {
                quizesQuery = quizesQuery.Where(s => s.Name.Contains(namePattern));
            }

            if (categoryId is not null)
            {
                quizesQuery = quizesQuery.Where(s => s.CategoryId == categoryId);
            }

            if (creatorId is not null)
            {
                quizesQuery = quizesQuery.Where(s => s.CreatorId == creatorId);
            }

            IEnumerable<QuizDTO> questionSets = quizesQuery;

            if (creatorFilter is not null)
            {
                int id = user.GetId();

                if (creatorFilter == CreatorFilter.Me)
                {
                    questionSets = questionSets.Where(s => s.CreatorId == id);
                }
                else if (creatorFilter == CreatorFilter.Friends)
                {
                    IEnumerable<QuizDTO> friendsQuestionSets = GetUserFriendsQuizes(id);

                    questionSets = questionSets.Intersect(friendsQuestionSets);
                }
                else throw new NotImplementedException();
            }

            IAsyncEnumerable<QuizDTO> quizesAsync = questionSets
                .ToArray()
                .ToAsyncEnumerable()
                .WhereAwait(async q => await authorizationService.CanUserAccessQuiz(user, q));

            quizesAsync = quizesAsync.Skip(limit * pageId).Take(limit);

            return quizesAsync;
        }

        public async Task<bool> IsNameConflict(string name)
        {
            return await Quizes.AnyAsync(q => q.Name == name);
        }

        public EntityEntry<QuizDTO> Add(QuizDTO quiz)
        {
            return Quizes.Add(quiz);
        }

        public EntityEntry<QuizDTO> Remove(QuizDTO quiz)
        {
            return Quizes.Remove(quiz);
        }
    }
}
