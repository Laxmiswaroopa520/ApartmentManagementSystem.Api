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
        public DbSet<UserRole> UserRoles => Set<UserRole>();//for many users can have many roles..

        public DbSet<Apartment> Apartments => Set<Apartment>();
        public DbSet<Flat> Flats => Set<Flat>();
        public DbSet<UserFlatMapping> UserFlatMappings => Set<UserFlatMapping>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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


            // STAFF MEMBER CONFIG
            
            modelBuilder.Entity<StaffMember>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FullName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Phone)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(e => e.Email)
                      .HasMaxLength(100);

                entity.Property(e => e.StaffType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.HourlyRate)
                      .HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Phone);
                entity.HasIndex(e => e.StaffType);
            });

       
            // MASTER DATA SEEDING
  
            SeedRoles(modelBuilder);
            SeedSuperAdmin(modelBuilder);
            SeedDemoApartment(modelBuilder);
            SeedFloors(modelBuilder);
            SeedFlats(modelBuilder);
        }

      
        // DEMO APARTMENT SEEDING
       
        private static void SeedDemoApartment(ModelBuilder modelBuilder)
        {
            var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<Apartment>().HasData(new Apartment
            {
                Id = apartmentId,
                Name = "Green Valley Apartments",
                Address = "123 Main Street, Chennai",
                TotalFlats = 200,
                CreatedAt = DateTime.UtcNow
            });
        }

        // FLOOR SEEDING
        private static void SeedFloors(ModelBuilder modelBuilder)
        {
            var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var floors = new List<Floor>();

            for (int i = 1; i <= 20; i++)
            {
                floors.Add(new Floor
                {
                    Id = Guid.Parse($"40000000-0000-0000-0000-{i:D12}"),
                    FloorNumber = i,
                    Name = $"Floor {i}",
                    ApartmentId = apartmentId
                });
            }

            modelBuilder.Entity<Floor>().HasData(floors);
        }

      
        // FLAT SEEDING
        private static void SeedFlats(ModelBuilder modelBuilder)
        {
            var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var flats = new List<Flat>();
            int flatCounter = 1;

            for (int floor = 1; floor <= 20; floor++)
            {
                var floorId = Guid.Parse($"40000000-0000-0000-0000-{floor:D12}");

                for (int flat = 1; flat <= 10; flat++)
                {
                    var flatNumber = $"{floor}{flat:D2}";

                    flats.Add(new Flat
                    {
                        Id = Guid.Parse($"50000000-0000-0000-0000-{flatCounter:D12}"),
                        FlatNumber = flatNumber,
                        Name = $"Flat {flatNumber}",
                        FloorId = floorId,
                        ApartmentId = apartmentId,
                        IsOccupied = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });

                    flatCounter++;
                }
            }

            modelBuilder.Entity<Flat>().HasData(flats);
        }

        
        // ROLE SEEDING

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
             //   RoleId = SystemRoleIds.SuperAdmin,
                IsActive = true,
                IsOtpVerified = true,
                IsRegistrationCompleted = true,
                CreatedAt = DateTime.UtcNow
            });
            // THIS is where role is assigned now
            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                UserId = adminUserId,
                RoleId = SystemRoleIds.SuperAdmin
            });
        }
    }
}






















/*
namespace ApartmentManagementSystem.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

      
        // PHASE 1 DB SETS
        
        public DbSet<Floor> Floors { get; set; }
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserOtp> UserOtps => Set<UserOtp>();
        public DbSet<UserInvite> UserInvites => Set<UserInvite>();
       public DbSet<StaffMember> StaffMembers { get; set; }
        // =========================
        // PHASE 2 DB SETS
        // =========================
        public DbSet<Apartment> Apartments => Set<Apartment>();
        public DbSet<Flat> Flats => Set<Flat>();
        public DbSet<UserFlatMapping> UserFlatMappings => Set<UserFlatMapping>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply Fluent configs
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // =========================
            // MASTER DATA SEEDING
            // =========================
            SeedRoles(modelBuilder);
            SeedSuperAdmin(modelBuilder);
            SeedDemoApartment(modelBuilder);
            SeedFloors(modelBuilder);
            SeedFlats(modelBuilder);
        }

        // =========================
        // DEMO APARTMENT SEEDING
        // =========================
        private static void SeedDemoApartment(ModelBuilder modelBuilder)
        {
            var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");

            modelBuilder.Entity<Apartment>().HasData(new Apartment
            {
                Id = apartmentId,
                Name = "Green Valley Apartments",
                Address = "123 Main Street, Chennai",
                TotalFlats = 200,
                CreatedAt = DateTime.UtcNow
            });
        }

        // =========================
        // FLOOR SEEDING
        // =========================
        private static void SeedFloors(ModelBuilder modelBuilder)
        {
            var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var floors = new List<Floor>();

            for (int i = 1; i <= 20; i++)
            {
                floors.Add(new Floor
                {
                    Id = Guid.Parse($"40000000-0000-0000-0000-{i:D12}"),
                    FloorNumber = i,
                    Name = $"Floor {i}",
                    ApartmentId = apartmentId
                });
            }

            modelBuilder.Entity<Floor>().HasData(floors);
        }

        // =========================
        // FLAT SEEDING
        // =========================
        private static void SeedFlats(ModelBuilder modelBuilder)
        {
            var apartmentId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var flats = new List<Flat>();
            int flatCounter = 1;

            for (int floor = 1; floor <= 20; floor++)
            {
                var floorId = Guid.Parse($"40000000-0000-0000-0000-{floor:D12}");
                for (int flat = 1; flat <= 10; flat++)
                {
                    var flatNumber = $"{floor}{flat:D2}";

                    flats.Add(new Flat
                    {
                        Id = Guid.Parse($"50000000-0000-0000-0000-{flatCounter:D12}"),
                        FlatNumber = flatNumber,
                        Name = $"Flat {flatNumber}",
                        FloorId = floorId,
                        ApartmentId = apartmentId, // ✅ FIXED
                        IsOccupied = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });

                    flatCounter++;
                }
            }

            modelBuilder.Entity<Flat>().HasData(flats);
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
    }
}


*/





