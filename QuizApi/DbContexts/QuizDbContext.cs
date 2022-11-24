using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuizApi.DTOs;

namespace QuizApi.DbContexts
{
    public class QuizDbContext : DbContext
    {
        public DbSet<QuestionDTO> Questions { get; set; }

        public DbSet<QuizDTO> QuestionSets { get; set; }

        public DbSet<CategoryDTO> QuestionSetCategories { get; set; }

        public DbSet<UserDTO> Users { get; set; }

        public DbSet<FriendshipDTO> Friendships { get; set; }

        public DbSet<FriendshipRequestDTO> FriendshipRequests { get; set; }

        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDTO>()
                .HasMany(u => u.Friendships)
                .WithOne(f => f.Me)
                .HasForeignKey(f => f.MeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDTO>()
                .HasMany(u => u.FriendshipRequests)
                .WithOne(f => f.Sender)
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendshipDTO>().HasKey(f => new { f.MeId, f.TheyId });

            modelBuilder.Entity<FriendshipRequestDTO>().HasKey(f => new { f.SenderId, f.ReceiverId });

            base.OnModelCreating(modelBuilder);
        }

        private void OnSaveChanges()
        {
            IEnumerable<object> added = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            foreach (UserDTO user in added.OfType<UserDTO>())
            {
                user.JoinDate = DateTime.Today;
            }

            foreach (QuizDTO questionSet in added.OfType<QuizDTO>())
            {
                questionSet.CreationDate = DateTime.Today;
            }
        }

        public override int SaveChanges()
        {
            OnSaveChanges();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnSaveChanges();

            return base.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserDTO?> FindUserByName(string name)
        {
            return await Users.Where(user => user.Name! == name).SingleOrDefaultAsync();
        }

        public async Task<bool> AreUsersFriends(int userId1, int userId2)
        {
            return await Friendships.FindAsync(userId1, userId2) != null || await Friendships.FindAsync(userId2, userId1) != null;
        }

        public IEnumerable<QuizDTO> GetUserFriendsQuestionSets(int userId)
        {
            return Friendships
                .Where(f => f.MeId == userId || f.TheyId == userId)
                .Join(QuestionSets, f => f.MeId == userId ? f.TheyId : f.MeId, qs => qs.CreatorId, (_, qs) => qs);
        }
    }
}
