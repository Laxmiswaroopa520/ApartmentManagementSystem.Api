// ApartmentManagementSystem.Infrastructure/Persistence/DatabaseSeeder.cs
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;

    public DatabaseSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Seed all roles from UserRole enum
        await SeedRolesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var rolesToSeed = new List<(string Name, string Description)>
        {
            // System Roles
            (RoleNames.SuperAdmin, "Super Administrator with full system access"),
            (RoleNames.Manager, "Manager with administrative privileges"),
            
            // Community Roles
            (RoleNames.President, "Community President - Leadership role"),
            (RoleNames.Secretary, "Community Secretary - Administrative role"),
            (RoleNames.Treasurer, "Community Treasurer - Financial management role"),
            
            // Resident Roles
            (RoleNames.ResidentOwner, "Resident Owner of the flat"),
            (RoleNames.Tenant, "Tenant renting the flat"),
            
            // Staff Roles
            (RoleNames.Security, "Security personnel"),
            (RoleNames.Plumber, "Plumber staff"),
            (RoleNames.Electrician, "Electrician staff"),
            (RoleNames.Carpenter, "Carpenter staff"),
            (RoleNames.Sweeper, "Cleaning staff"),
            (RoleNames.Gardener, "Gardening staff"),
            (RoleNames.MaintenanceStaff, "General maintenance staff")
        };

        foreach (var (name, description) in rolesToSeed)
        {
            var exists = await _context.Roles.AnyAsync(r => r.Name == name);

            if (!exists)
            {
                _context.Roles.Add(new Role
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}