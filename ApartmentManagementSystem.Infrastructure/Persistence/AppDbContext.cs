
// Infrastructure/Persistence/AppDbContext.cs
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DB SETS
        public DbSet<Floor> Floors { get; set; }
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserOtp> UserOtps => Set<UserOtp>();
        public DbSet<UserInvite> UserInvites => Set<UserInvite>();
        public DbSet<StaffMember> StaffMembers { get; set; }
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Apartment> Apartments => Set<Apartment>();
        public DbSet<Flat> Flats => Set<Flat>();
        public DbSet<UserFlatMapping> UserFlatMappings => Set<UserFlatMapping>();
        public DbSet<ApartmentManager> ApartmentManagers => Set<ApartmentManager>();
        public DbSet<CommunityMember> CommunityMembers => Set<CommunityMember>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Apply all entity configurations from separate files
            // This will automatically apply all IEntityTypeConfiguration<T> classes
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // ⭐ SEED MASTER DATA
            SeedRoles(modelBuilder);
            SeedSuperAdmin(modelBuilder);
        }

        // ⭐ ROLE SEEDING
        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = SystemRoleIds.SuperAdmin, Name = SystemRoles.SuperAdmin, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Manager, Name = SystemRoles.Manager, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.President, Name = SystemRoles.President, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Secretary, Name = SystemRoles.Secretary, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Treasurer, Name = SystemRoles.Treasurer, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.ResidentOwner, Name = SystemRoles.ResidentOwner, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Tenant, Name = SystemRoles.Tenant, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Security, Name = SystemRoles.Security, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Maintenance, Name = SystemRoles.Maintenance, CreatedAt = DateTime.UtcNow }
            );
        }

        // ⭐ SUPER ADMIN SEEDING
        private static void SeedSuperAdmin(ModelBuilder modelBuilder)
        {
            var adminUserId = Guid.Parse("20000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminUserId,
                FullName = "System Administrator",
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PrimaryPhone = "9999999999",
                Email = "admin@apartment.com",
                IsActive = true,
                IsOtpVerified = true,
                IsRegistrationCompleted = true,
                CreatedAt = DateTime.UtcNow
            });

            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = adminUserId,
                RoleId = SystemRoleIds.SuperAdmin,
                AssignedAt = DateTime.UtcNow
            });
        }
    }
}
