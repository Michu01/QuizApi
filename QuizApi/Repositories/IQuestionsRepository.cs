using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public interface IQuestionsRepository : IRepository
    {
        Task<QuestionDTO?> Find(int id);

        EntityEntry<QuestionDTO> Add(QuestionDTO question);

        EntityEntry<QuestionDTO> Remove(QuestionDTO question);

        IEnumerable<QuestionDTO> FindByQuizId(int quizId);
    }
}
