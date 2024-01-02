using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set primary key
            modelBuilder.Entity<UserLike>().HasKey(k => new { k.SourceUserId, k.TargetUserId });

            // 1 user can like many users
            modelBuilder.Entity<UserLike>().HasOne(s => s.SourceUser).WithMany(l => l.LikedUsers).HasForeignKey(s => s.SourceUserId).OnDelete(DeleteBehavior.Cascade);
            // 1 user can be liked by many users
            modelBuilder.Entity<UserLike>().HasOne(s => s.TargetUser).WithMany(l => l.LikedByUsers).HasForeignKey(s => s.TargetUserId).OnDelete(DeleteBehavior.Cascade);

            // 1 user can receive many messages
            modelBuilder.Entity<Message>().HasOne(u => u.Recipient).WithMany(m => m.MessagesReceived).OnDelete(DeleteBehavior.Restrict);

            // 1 user can send many messages
            modelBuilder.Entity<Message>().HasOne(u => u.Sender).WithMany(m => m.MessagesSent).OnDelete(DeleteBehavior.Restrict);
        }

    }
}