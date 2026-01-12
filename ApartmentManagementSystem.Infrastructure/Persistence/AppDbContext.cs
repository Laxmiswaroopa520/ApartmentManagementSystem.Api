using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserOtp> UserOtps => Set<UserOtp>();
        public DbSet<UserInvite> UserInvites => Set<UserInvite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // ROLE MASTER DATA
            // =========================
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = SystemRoleIds.SuperAdmin, Name = SystemRoles.SuperAdmin, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.President, Name = SystemRoles.President, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Secretary, Name = SystemRoles.Secretary, CreatedAt = DateTime.UtcNow },

                new Role { Id = SystemRoleIds.ResidentOwner, Name = SystemRoles.ResidentOwner, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Tenant, Name = SystemRoles.Tenant, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Security, Name = SystemRoles.Security, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Maintenance, Name = SystemRoles.Maintenance, CreatedAt = DateTime.UtcNow }
            );

            // =========================
            // DEFAULT SUPER ADMIN USER
            // =========================
            var adminUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    RoleId = SystemRoleIds.SuperAdmin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
