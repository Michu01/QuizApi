using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using QuizApi.DbContexts;
using QuizApi.DTOs;
using QuizApi.Extensions;

namespace QuizApi.Repositories
{
    public class UsersRepository : Repository, IUsersRepository
    {
        private readonly IFriendshipsRepository friendshipsRepository;

        public UsersRepository(QuizDbContext dbContext, IFriendshipsRepository friendshipsRepository) : base(dbContext) 
        {
            this.friendshipsRepository = friendshipsRepository;
        }

        private DbSet<UserDTO> Users => DbContext.Users;

        public EntityEntry<UserDTO> Add(UserDTO user)
        {
            return Users.Add(user);
        }

        public async Task<UserDTO?> Find(int id)
        {
            return await Users.FindAsync(id);
        }

        public async Task<UserDTO?> FindByName(string name)
        {
            return await Users.Where(user => user.Name! == name).SingleOrDefaultAsync();
        }

        public IAsyncEnumerable<UserDTO> Get(int pageId, int limit, string? namePattern, bool friendsOnly, int? userId)
        {
            IQueryable<UserDTO> usersQuery = Users;

            if (!string.IsNullOrEmpty(namePattern))
            {
                usersQuery = usersQuery.Where(u => u.Name.Contains(namePattern));
            }

            IAsyncEnumerable<UserDTO> users = usersQuery.ToArray().ToAsyncEnumerable();

            if (friendsOnly)
            {
                if (userId is null)
                {
                    throw new Exception("User must be signed in to view friends");
                }

                users = users.WhereAwait(async u => await friendshipsRepository.AreUsersFriends(userId.Value, u.Id));
            }

            users = users.Skip(pageId * limit).Take(limit);

            return users;
        }

        public async Task<IEnumerable<UserDTO>> GetFriends(int userId)
        {
            if (await Find(userId) is not UserDTO user)
            {
                throw new ArgumentException($"User with id: {userId} not found", nameof(userId));
            }

            await DbContext.Entry(user).Collection(u => u.Friendships).LoadAsync();

            foreach (FriendshipDTO friendship in user.Friendships)
            {
                await DbContext.Entry(friendship).Reference(f => f.FirstUserId == userId ? f.SecondUser : f.FirstUser).LoadAsync();
            }

            return user.Friendships.Select(f => f.FirstUserId == userId ? f.SecondUser : f.FirstUser);
        }
    }
}
