using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Infrastructure.Persistence
{
    using ApartmentManagementSystem.Domain.Entities;
    using global::ApartmentManagementSystem.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

   // namespace ApartmentManagementSystem.Infrastructure.Persistence;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserOtp> UserOtps => Set<UserOtp>();
        public DbSet<UserInvite> UserInvites => Set<UserInvite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "SuperAdmin", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "President", CreatedAt = DateTime.UtcNow },
                new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Secretary", CreatedAt = DateTime.UtcNow }
            );
        }
    }
}