using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Infrastructure.Persistence
{
    using ApartmentManagementSystem.Domain.Entities;
   // using global::ApartmentManagementSystem.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

   // namespace ApartmentManagementSystem.Infrastructure.Persistence;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserOtp> UserOtps => Set<UserOtp>();
        public DbSet<UserInvite> UserInvites => Set<UserInvite>();

        /*   protected override void OnModelCreating(ModelBuilder modelBuilder)
           {
               base.OnModelCreating(modelBuilder);

               modelBuilder.Entity<Role>().HasData(
                   new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "SuperAdmin", CreatedAt = DateTime.UtcNow },
                   new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "President", CreatedAt = DateTime.UtcNow },
                   new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Secretary", CreatedAt = DateTime.UtcNow }
               );
           }*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // ROLES (MASTER DATA)
            // =========================
            var superAdminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var presidentRoleId = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var secretaryRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");
            modelBuilder.Entity<UserInvite>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.PrimaryPhone)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(x => x.InviteStatus)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.Property(x => x.CreatedByUserId)
                      .IsRequired();

                entity.Property(x => x.UserId)
                      .IsRequired();

                entity.Property(x => x.ExpiresAt)
                      .IsRequired();

                entity.HasOne(x => x.Role)
                      .WithMany()
                      .HasForeignKey(x => x.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = superAdminRoleId,
                    Name = "SuperAdmin",
                    CreatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = presidentRoleId,
                    Name = "President",
                    CreatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = secretaryRoleId,
                    Name = "Secretary",
                    CreatedAt = DateTime.UtcNow
                }
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
                    RoleId = superAdminRoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }

    }
}