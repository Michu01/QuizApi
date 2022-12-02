using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public interface ICategoriesRepository : IRepository
    {
        IEnumerable<CategoryDTO> Get();

        Task<CategoryDTO?> Find(int id);

        EntityEntry<CategoryDTO> Add(CategoryDTO category);

        EntityEntry<CategoryDTO> Remove(CategoryDTO category);
    }
}
