using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DbContexts;
using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public class CategoriesRepository : Repository, ICategoriesRepository
    {
        private DbSet<CategoryDTO> Categories => DbContext.Categories;

        public CategoriesRepository(QuizDbContext dbContext) : base(dbContext) {}

        public IEnumerable<CategoryDTO> Get()
        {
            return Categories;
        }

        public async Task<CategoryDTO?> Find(int id)
        {
            return await Categories.FindAsync(id);
        }

        public EntityEntry<CategoryDTO> Add(CategoryDTO category)
        {
            return Categories.Add(category);
        }

        public EntityEntry<CategoryDTO> Remove(CategoryDTO category)
        {
            return Categories.Remove(category);
        }
    }
}
