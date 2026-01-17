using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // =========================
        // PHASE 1 DB SETS
        // =========================
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserOtp> UserOtps => Set<UserOtp>();
        public DbSet<UserInvite> UserInvites => Set<UserInvite>();

        // =========================
        // PHASE 2 DB SETS
        // =========================
        public DbSet<Apartment> Apartments => Set<Apartment>();
        public DbSet<Flat> Flats => Set<Flat>();
        public DbSet<UserFlatMapping> UserFlatMappings => Set<UserFlatMapping>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // FLAT RELATIONSHIPS (NO CASCADE PATH ISSUES)
            // =========================
            /*  modelBuilder.Entity<Flat>(entity =>
              {
                  entity.HasOne(f => f.Apartment)
                      .WithMany(a => a.Flats)
                      .HasForeignKey(f => f.ApartmentId)
                      .OnDelete(DeleteBehavior.NoAction);

                  entity.HasOne(f => f.OwnerUser)
                      .WithMany()
                      .HasForeignKey(f => f.OwnerUserId)
                      .OnDelete(DeleteBehavior.NoAction);

                  entity.HasOne(f => f.TenantUser)
                      .WithMany()
                      .HasForeignKey(f => f.TenantUserId)
                      .OnDelete(DeleteBehavior.NoAction);
              });
            */
           /* modelBuilder.Entity<Flat>(entity =>
            {
                entity.HasOne(f => f.Apartment)
                    .WithMany(a => a.Flats)
                    .HasForeignKey(f => f.ApartmentId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(f => f.OwnerUser)
                    .WithMany()
                    .HasForeignKey(f => f.OwnerUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(f => f.TenantUser)
                    .WithMany()
                    .HasForeignKey(f => f.TenantUserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });*/

            // Apply other Fluent configs
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            SeedRoles(modelBuilder);
            SeedSuperAdmin(modelBuilder);
            SeedDemoApartment(modelBuilder);
        }

        // =========================
        // ROLE SEEDING
        // =========================
        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = SystemRoleIds.SuperAdmin, Name = SystemRoles.SuperAdmin, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.President, Name = SystemRoles.President, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Secretary, Name = SystemRoles.Secretary, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Treasurer, Name = SystemRoles.Treasurer, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.ResidentOwner, Name = SystemRoles.ResidentOwner, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Tenant, Name = SystemRoles.Tenant, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Security, Name = SystemRoles.Security, CreatedAt = DateTime.UtcNow },
                new Role { Id = SystemRoleIds.Maintenance, Name = SystemRoles.Maintenance, CreatedAt = DateTime.UtcNow }
            );
        }

        // =========================
        // SUPER ADMIN SEEDING
        // =========================
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
                RoleId = SystemRoleIds.SuperAdmin,
                IsActive = true,
                IsOtpVerified = true,
                IsRegistrationCompleted = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // =========================
        // DEMO APARTMENT
        // =========================
        private static void SeedDemoApartment(ModelBuilder modelBuilder)
        {
            var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<Apartment>().HasData(new Apartment
            {
                Id = apartmentId,
                Name = "Green Valley Apartments",
                Address = "123 Main Street, Chennai",
                TotalFlats = 0,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}







































/*using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    // =========================
    // PHASE 1 DB SETS
    // =========================
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserOtp> UserOtps => Set<UserOtp>();
    public DbSet<UserInvite> UserInvites => Set<UserInvite>();

    // =========================
    // PHASE 2 DB SETS
    // =========================
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Flat> Flats => Set<Flat>();
    public DbSet<UserFlatMapping> UserFlatMappings => Set<UserFlatMapping>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // FIX: MULTIPLE CASCADE PATH ISSUE
        // =========================
        modelBuilder.Entity<Flat>(entity =>
        {
            entity.HasOne<Apartment>()
                .WithMany()
                .HasForeignKey(f => f.ApartmentId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.OwnerUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.TenantUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // Apply Fluent API configurations (Phase 2+)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        SeedRoles(modelBuilder);
        SeedSuperAdmin(modelBuilder);
        SeedDemoApartment(modelBuilder);
    }

    // =========================
    // ROLE SEEDING
    // =========================
    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = SystemRoleIds.SuperAdmin, Name = SystemRoles.SuperAdmin, CreatedAt = DateTime.UtcNow },
            new Role { Id = SystemRoleIds.President, Name = SystemRoles.President, CreatedAt = DateTime.UtcNow },
            new Role { Id = SystemRoleIds.Secretary, Name = SystemRoles.Secretary, CreatedAt = DateTime.UtcNow },
            new Role { Id = SystemRoleIds.Treasurer, Name = SystemRoles.Treasurer, CreatedAt = DateTime.UtcNow },

            new Role { Id = SystemRoleIds.ResidentOwner, Name = SystemRoles.ResidentOwner, CreatedAt = DateTime.UtcNow },
            new Role { Id = SystemRoleIds.Tenant, Name = SystemRoles.Tenant, CreatedAt = DateTime.UtcNow },
            new Role { Id = SystemRoleIds.Security, Name = SystemRoles.Security, CreatedAt = DateTime.UtcNow },
            new Role { Id = SystemRoleIds.Maintenance, Name = SystemRoles.Maintenance, CreatedAt = DateTime.UtcNow }
        );
    }

    // =========================
    // SUPER ADMIN SEEDING
    // =========================
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
            RoleId = SystemRoleIds.SuperAdmin,
            IsActive = true,
            IsOtpVerified = true,
            IsRegistrationCompleted = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    // =========================
    // PHASE 2 DEMO DATA
    // =========================
    private static void SeedDemoApartment(ModelBuilder modelBuilder)
    {
        var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");

        modelBuilder.Entity<Apartment>().HasData(new Apartment
        {
            Id = apartmentId,
            Name = "Green Valley Apartments",
            Address = "123 Main Street, Chennai",
            TotalFlats = 0,
            CreatedAt = DateTime.UtcNow
        });
    }
}





*/












