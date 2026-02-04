// Infrastructure/Persistence/AppDbContext.cs
/*using ApartmentManagementSystem.Domain.Constants;
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
        public DbSet<ApartmentManager> ApartmentManagers => Set<ApartmentManager>(); // ⭐ NEW
        public DbSet<CommunityMember> CommunityMembers => Set<CommunityMember>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            // USER ↔ ROLE MANY-TO-MANY
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Apply Fluent configs from IEntityTypeConfiguration<>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // APARTMENT CONFIGURATION
            modelBuilder.Entity<Apartment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(100);
                entity.Property(e => e.PinCode).HasMaxLength(10);
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsActive);
            });

            // APARTMENT MANAGER CONFIGURATION ⭐
            modelBuilder.Entity<ApartmentManager>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Apartment)
                    .WithMany(a => a.Managers)
                    .HasForeignKey(e => e.ApartmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.ApartmentId, e.IsActive });
            });

            // COMMUNITY MEMBER CONFIGURATION (Updated) ⭐
            modelBuilder.Entity<CommunityMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CommunityRole).IsRequired().HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Apartment)
                    .WithMany(a => a.CommunityMembers)
                    .HasForeignKey(e => e.ApartmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ApartmentId, e.CommunityRole, e.IsActive });
            });

            // STAFF MEMBER CONFIG
            modelBuilder.Entity<StaffMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.StaffType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Phone);
                entity.HasIndex(e => e.StaffType);
            });

            // MASTER DATA SEEDING - NO LONGER SEED APARTMENTS HERE
            SeedRoles(modelBuilder);
            SeedSuperAdmin(modelBuilder);
            // ⭐ REMOVED: SeedDemoApartment, SeedFloors, SeedFlats
            // Now apartments will be created dynamically via UI
        }

        // ROLE SEEDING
        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = SystemRoleIds.SuperAdmin, Name = SystemRoles.SuperAdmin, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Manager, Name = SystemRoles.Manager, CreatedAt = DateTime.UtcNow }, // ⭐ ADD Manager
                new Role { Id = SystemRoleIds.President, Name = SystemRoles.President, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Secretary, Name = SystemRoles.Secretary, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Treasurer, Name = SystemRoles.Treasurer, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.ResidentOwner, Name = SystemRoles.ResidentOwner, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Tenant, Name = SystemRoles.Tenant, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Security, Name = SystemRoles.Security, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Maintenance, Name = SystemRoles.Maintenance, CreatedAt = DateTime.UtcNow }
            );
        }

        // SUPER ADMIN SEEDING
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
                UserId = adminUserId,
                RoleId = SystemRoleIds.SuperAdmin
            });
        }
    }
}
*/


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
