using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public interface IFriendshipRequestsRepository : IRepository
    {
        Task<FriendshipRequestDTO?> Find(int senderId, int receiverId);

        IEnumerable<FriendshipRequestDTO> Get(int id);

        IEnumerable<FriendshipRequestDTO> GetReceived(int id);

        IEnumerable<FriendshipRequestDTO> GetSent(int id);

        Task<bool> IsInvited(int firstId, int secondId);

        EntityEntry<FriendshipRequestDTO> Add(FriendshipRequestDTO friendshipRequest);

        EntityEntry<FriendshipRequestDTO> Remove(FriendshipRequestDTO friendshipRequest);
    }
}
