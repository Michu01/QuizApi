using QuizApi.DbContexts;

namespace QuizApi.Repositories
{
    public abstract class Repository : IRepository
    {
        public QuizDbContext DbContext { get; }

        public Repository(QuizDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }
}
