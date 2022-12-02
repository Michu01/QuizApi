namespace QuizApi.Services
{
    public interface IAvatarService
    {
        string? GetPath(int userId);

        Task<string> Change(IFormFile file, int userId);
    }
}
