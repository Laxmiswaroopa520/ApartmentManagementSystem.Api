using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ApartmentManagementSystem.Domain.Entities;
namespace ApartmentManagementSystem.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Use in-memory configuration for testing
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["JwtSettings:SecretKey"] = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm!",
                ["JwtSettings:Issuer"] = "ApartmentManagementSystem",
                ["JwtSettings:Audience"] = "ApartmentManagementSystemUsers",
                ["JwtSettings:ExpireMinutes"] = "60"
            }!);
        });

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext using an in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTestDb");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();

                // Seed test data if needed
                SeedTestData(db);
            }
        });
    }

    private void SeedTestData(AppDbContext context)
    {
        // Clear existing data
        context.Users.RemoveRange(context.Users);
        context.Roles.RemoveRange(context.Roles);
        context.UserRoles.RemoveRange(context.UserRoles);
        context.UserInvites.RemoveRange(context.UserInvites);
        context.UserOtps.RemoveRange(context.UserOtps);
        context.SaveChanges();

        // Seed roles
        var roles = new[]
        {
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "SuperAdmin", CreatedAt = DateTime.UtcNow },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Manager", CreatedAt = DateTime.UtcNow },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "ResidentOwner", CreatedAt = DateTime.UtcNow },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Tenant", CreatedAt = DateTime.UtcNow },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Staff", CreatedAt = DateTime.UtcNow },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "President", CreatedAt = DateTime.UtcNow }
        };
        context.Roles.AddRange(roles);

        // Seed test SuperAdmin user
        var adminUserId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var adminUser = new User
        {
            Id = adminUserId,
            FullName = "Test Admin",
            Username = "testadmin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            PrimaryPhone = "9999999999",
            Email = "admin@test.com",
            IsActive = true,
            IsOtpVerified = true,
            IsRegistrationCompleted = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(adminUser);

        // Assign SuperAdmin role
        context.UserRoles.Add(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = adminUserId,
            RoleId = roles[0].Id,
            AssignedAt = DateTime.UtcNow
        });

        // Seed test regular user
        var testUserId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var testUser = new User
        {
            Id = testUserId,
            FullName = "Test Owner",
            Username = "testowner",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Owner@123"),
            PrimaryPhone = "8888888888",
            Email = "owner@test.com",
            IsActive = true,
            IsOtpVerified = true,
            IsRegistrationCompleted = true,
            ResidentType = Domain.Enums.ResidentType.Owner,
            Status = Domain.Enums.ResidentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(testUser);

        // Assign ResidentOwner role
        context.UserRoles.Add(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = roles[2].Id,
            AssignedAt = DateTime.UtcNow
        });

        // Seed inactive user for testing
        var inactiveUserId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var inactiveUser = new User
        {
            Id = inactiveUserId,
            FullName = "Inactive User",
            Username = "inactiveuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Inactive@123"),
            PrimaryPhone = "7777777777",
            Email = "inactive@test.com",
            IsActive = false,
            IsOtpVerified = true,
            IsRegistrationCompleted = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(inactiveUser);

        context.UserRoles.Add(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = inactiveUserId,
            RoleId = roles[2].Id,
            AssignedAt = DateTime.UtcNow
        });

        context.SaveChanges();
    }
}