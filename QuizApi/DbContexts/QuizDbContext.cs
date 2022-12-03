using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuizApi.DTOs;

namespace QuizApi.DbContexts
{
    public class QuizDbContext : DbContext
    {
        public virtual DbSet<QuestionDTO> Questions { get; set; }

        public virtual DbSet<QuizDTO> Quizes { get; set; }

        public virtual DbSet<CategoryDTO> Categories { get; set; }

        public virtual DbSet<UserDTO> Users { get; set; }

        public virtual DbSet<FriendshipDTO> Friendships { get; set; }

        public virtual DbSet<FriendshipRequestDTO> FriendshipRequests { get; set; }

        public QuizDbContext() { }

        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDTO>()
                .HasMany(u => u.Friendships)
                .WithOne(f => f.FirstUser)
                .HasForeignKey(f => f.FirstUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDTO>()
                .HasMany(u => u.FriendshipRequests)
                .WithOne(f => f.Sender)
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FriendshipDTO>().HasKey(f => new { f.FirstUserId, f.SecondUserId });

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
    }
}