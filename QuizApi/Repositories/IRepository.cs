namespace QuizApi.Repositories
{
    public interface IRepository
    {
        Task<int> SaveChangesAsync();
    }
}
