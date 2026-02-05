using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// web application factory::is a class from microsoft.aspnetcore,testing
//spins your entire app in memory
//creates an http client to make requrests
//runs tests without deploying it to a real server..
namespace ApartmentManagementSystem.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? Connection;                                                                 //exists only in ram ;no need of sql server;;each test run starts with clean data
                                                                                                         //automtically destoryed after tests

    protected override void ConfigureWebHost(IWebHostBuilder builder)                           //overrides how the web app is configured for testing::called automatically by xunit when the factory is created
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>                     //override
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["JwtSettings:SecretKey"] = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm!",
                ["JwtSettings:Issuer"] = "ApartmentManagementSystem",
                ["JwtSettings:Audience"] = "ApartmentManagementSystemUsers",
                ["JwtSettings:ExpireMinutes"] = "60"
            }!);
        });

        builder.ConfigureServices(services =>
        {                                                                               //finds the existing configuration and removes it from di
            // Remove existing DbContext    :: we can replace it with sqllite
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Create a shared in-memory SQLite database connection
            Connection = new SqliteConnection("DataSource=:memory:");
            Connection.Open();              //in memory databases open when the connection is open

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(Connection);
                // Enable sensitive data logging for debugging
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is created with schema
            db.Database.EnsureCreated();

            // Seed test data
            SeedTestData(db);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Connection?.Close();
            Connection?.Dispose();
        }
        base.Dispose(disposing);
    }

    private static void SeedTestData(AppDbContext context)
    {
        // Check if already seeded
        if (context.Roles.Any())
            return;

        // Seed Roles
        var roles = new[]
        {
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "SuperAdmin",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Name = "Manager",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "ResidentOwner",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "Tenant",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name = "Staff",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                Name = "President",
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Roles.AddRange(roles);
        context.SaveChanges();

        // Seed Admin User
        var adminId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var adminUser = new Domain.Entities.User
        {
            Id = adminId,
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
        context.SaveChanges();

        // Assign Admin Role
        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = adminId,
            RoleId = roles[0].Id,
            AssignedAt = DateTime.UtcNow
        });
        context.SaveChanges();

        // Seed Regular User
        var testUserId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var testUser = new Domain.Entities.User
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
        context.SaveChanges();

        // Assign ResidentOwner role
        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = roles[2].Id,
            AssignedAt = DateTime.UtcNow
        });
        context.SaveChanges();

        // Seed Inactive User
        var inactiveUserId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var inactiveUser = new Domain.Entities.User
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
        context.SaveChanges();

        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = inactiveUserId,
            RoleId = roles[2].Id,
            AssignedAt = DateTime.UtcNow
        });
        context.SaveChanges();
    }
}


















/*using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApartmentManagementSystem.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
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
            // Remove existing DbContext
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(descriptor);

            // Create unique in-memory SQLite database per test class
            _connection = new SqliteConnection($"DataSource=file:{Guid.NewGuid()}?mode=memory&cache=shared");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
                // Optional: Enable sensitive data logging for debugging
                // options.EnableSensitiveDataLogging();
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply migrations (this will use the new SQLite-compatible migration)
            db.Database.Migrate();

            // Seed test data
            SeedTestData(db);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }

    private static void SeedTestData(AppDbContext context)
    {
        // Don't seed if already seeded
        if (context.Roles.Any())
            return;

        // Seed Roles
        var roles = new[]
        {
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Name = "SuperAdmin",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                Name = "ResidentOwner",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                Name = "Tenant",
                CreatedAt = DateTime.UtcNow
            },
            new Domain.Entities.Role
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                Name = "Staff",
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Roles.AddRange(roles);
        context.SaveChanges();

        // Seed Admin User
        var adminId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var adminUser = new Domain.Entities.User
        {
            Id = adminId,
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
        context.SaveChanges();

        // Assign Admin Role
        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = adminId,
            RoleId = roles[0].Id,
            AssignedAt = DateTime.UtcNow
        });

        // Seed Inactive User for testing
        var inactiveUserId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var inactiveUser = new Domain.Entities.User
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
        context.SaveChanges();

        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = inactiveUserId,
            RoleId = roles[1].Id,
            AssignedAt = DateTime.UtcNow
        });

        context.SaveChanges();
    }
}


*/










/*using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace ApartmentManagementSystem.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    //   private SqliteConnection? _connection;
    private SqliteConnection _connection = null!;

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
            builder.UseEnvironment("Testing");

            // Create and open a connection to SQLite in-memory database
            // Connection must stay open for the lifetime of the test
            _connection = new SqliteConnection("DataSource=:memory:");
           // _connection = new SqliteConnection("DataSource=file:memdb_" + Guid.NewGuid() + "?mode=memory&cache=shared");            //Each factory gets its own isolated DB

            _connection.Open();

            // Add DbContext using SQLite in-memory database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Ensure the database is created with schema
                // db.Database.EnsureCreated();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();


                // Seed test data
                SeedTestData(db);
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void SeedTestData(AppDbContext context)
    {
        // Don't clear - just check if data already exists
        if (context.Roles.Any())
        {
            return; // Already seeded
        }

        // Seed roles
        var roles = new[]
        {
            new Domain.Entities.Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "SuperAdmin", CreatedAt = DateTime.UtcNow },
            new Domain.Entities.Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Manager", CreatedAt = DateTime.UtcNow },
            new Domain.Entities.Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "ResidentOwner", CreatedAt = DateTime.UtcNow },
            new Domain.Entities.Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "Tenant", CreatedAt = DateTime.UtcNow },
            new Domain.Entities.Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "Staff", CreatedAt = DateTime.UtcNow },
            new Domain.Entities.Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "President", CreatedAt = DateTime.UtcNow }
        };
        context.Roles.AddRange(roles);
        context.SaveChanges();

        // Seed test SuperAdmin user
        var adminUserId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var adminUser = new Domain.Entities.User
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
        context.SaveChanges();

        // Assign SuperAdmin role
        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = adminUserId,
            RoleId = roles[0].Id,
            AssignedAt = DateTime.UtcNow
        });
        context.SaveChanges();

        // Seed test regular user
        var testUserId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var testUser = new Domain.Entities.User
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
        context.SaveChanges();

        // Assign ResidentOwner role
        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            RoleId = roles[2].Id,
            AssignedAt = DateTime.UtcNow
        });
        context.SaveChanges();

        // Seed inactive user for testing
        var inactiveUserId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var inactiveUser = new Domain.Entities.User
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
        context.SaveChanges();

        context.UserRoles.Add(new Domain.Entities.UserRole
        {
            Id = Guid.NewGuid(),
            UserId = inactiveUserId,
            RoleId = roles[2].Id,
            AssignedAt = DateTime.UtcNow
        });
        context.SaveChanges();
    }
}

*/












