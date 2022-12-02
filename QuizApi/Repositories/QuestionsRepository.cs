using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DbContexts;
using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public class QuestionsRepository : Repository, IQuestionsRepository
    {
        public QuestionsRepository(QuizDbContext dbContext) : base(dbContext) {}

        private DbSet<QuestionDTO> Questions => DbContext.Questions;

        public EntityEntry<QuestionDTO> Add(QuestionDTO question)
        {
            return Questions.Add(question);
        }

        public async Task<QuestionDTO?> Find(int id)
        {
            return await Questions.FindAsync(id);
        }

        public IEnumerable<QuestionDTO> FindByQuizId(int quizId)
        {
            return Questions.Where(q => q.QuizId == quizId);
        }

        public EntityEntry<QuestionDTO> Remove(QuestionDTO question)
        {
            return Questions.Remove(question);
        }
    }
}
