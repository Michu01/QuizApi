using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DbContexts;
using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public class FriendshipRequestsRepository : Repository, IFriendshipRequestsRepository
    {
        private DbSet<FriendshipRequestDTO> FriendshipRequests => DbContext.FriendshipRequests;

        public FriendshipRequestsRepository(QuizDbContext dbContext) : base(dbContext)
        {

        }

        public IEnumerable<FriendshipRequestDTO> Get(int id)
        {
            return FriendshipRequests.Where(r => r.ReceiverId == id || r.SenderId == id);
        }

        public IEnumerable<FriendshipRequestDTO> GetReceived(int id)
        {
            return FriendshipRequests.Where(r => r.ReceiverId == id);
        }

        public IEnumerable<FriendshipRequestDTO> GetSent(int id)
        {
            return FriendshipRequests.Where(r => r.SenderId == id);
        }

        public async Task<bool> IsInvited(int firstId, int secondId)
        {
            return await FriendshipRequests.FindAsync(firstId, secondId) is not null;
        }

        public EntityEntry<FriendshipRequestDTO> Add(FriendshipRequestDTO friendshipRequest)
        {
            return FriendshipRequests.Add(friendshipRequest);
        }

        public EntityEntry<FriendshipRequestDTO> Remove(FriendshipRequestDTO friendshipRequest)
        {
            return FriendshipRequests.Remove(friendshipRequest);
        }

        public async Task<FriendshipRequestDTO?> Find(int senderId, int receiverId)
        {
            return await FriendshipRequests.FindAsync(senderId, receiverId);
        }
    }
}
