using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DbContexts;
using QuizApi.DTOs;

namespace QuizApi.Repositories
{
    public class FriendshipsRepository : Repository, IFriendshipsRepository
    {
        public FriendshipsRepository(QuizDbContext dbContext) : base(dbContext)
        {
        }

        private DbSet<FriendshipDTO> Friendships => DbContext.Friendships;

        public EntityEntry<FriendshipDTO> Add(FriendshipDTO friendship)
        {
            return Friendships.Add(friendship);
        }

        public EntityEntry<FriendshipDTO> Remove(FriendshipDTO friendship)
        {
            return Friendships.Remove(friendship);
        }

        public async Task<bool> AreUsersFriends(int firstId, int secondId)
        {
            return (await Friendships.FindAsync(firstId, secondId) ?? await Friendships.FindAsync(secondId, firstId)) is not null;
        }

        public IEnumerable<FriendshipDTO> Get(int id)
        {
            return Friendships.Where(f => f.FirstUserId == id || f.SecondUserId == id);
        }

        public async Task<FriendshipDTO?> Find(int firstId, int secondId)
        {
            return await Friendships.FindAsync(firstId, secondId);
        }
    }
}
