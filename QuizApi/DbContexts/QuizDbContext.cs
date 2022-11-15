﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuizApi.DTOs;

namespace QuizApi.DbContexts
{
    public class QuizDbContext : DbContext
    {
        public DbSet<QuestionDTO> Questions { get; set; }

        public DbSet<QuestionSetDTO> QuestionSets { get; set; }

        public DbSet<QuestionSetCategoryDTO> QuestionSetCategories { get; set; }

        public DbSet<UserDTO> Users { get; set; }

        public DbSet<FriendshipDTO> Friendships { get; set; }

        public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDTO>()
                .HasMany(u => u.Friendships)
                .WithOne(f => f.Me)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendshipDTO>().HasKey(f => new { f.MeId, f.TheyId });

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

            foreach (QuestionSetDTO questionSet in added.OfType<QuestionSetDTO>())
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
    }
}
