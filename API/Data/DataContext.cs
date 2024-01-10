using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1 user can has many roles
            modelBuilder.Entity<AppUser>().HasMany(ur => ur.UserRoles).WithOne(u => u.User).HasForeignKey(ur => ur.UserId).IsRequired();
            // 1 role can set to many users
            modelBuilder.Entity<AppRole>().HasMany(ur => ur.UserRoles).WithOne(r => r.Role).HasForeignKey(ur => ur.RoleId).IsRequired();

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

            // apply for all queries that targeting Photo entity
            modelBuilder.Entity<Photo>().HasQueryFilter(p => p.IsApproved == true);
        }

    }
}