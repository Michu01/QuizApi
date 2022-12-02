using System.Security.Claims;

using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public interface IUsersRepository : IRepository
    {
        IAsyncEnumerable<UserDTO> Get(
            int pageId,
            int limit,
            string? namePattern,
            bool friendsOnly,
            int? userId);

        Task<IEnumerable<UserDTO>> GetFriends(int userId);

        Task<UserDTO?> Find(int id);

        Task<UserDTO?> FindByName(string name);

        EntityEntry<UserDTO> Add(UserDTO user);
    }
}
