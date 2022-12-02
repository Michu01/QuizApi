using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public interface IFriendshipsRepository : IRepository
    {
        Task<FriendshipDTO?> Find(int firstId, int secondId);

        IEnumerable<FriendshipDTO> Get(int id);

        Task<bool> AreUsersFriends(int firstId, int secondId);

        EntityEntry<FriendshipDTO> Add(FriendshipDTO friendship);

        EntityEntry<FriendshipDTO> Remove(FriendshipDTO friendship);
    }
}
