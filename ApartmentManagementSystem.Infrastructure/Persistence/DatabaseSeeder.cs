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
            (UserRole.SuperAdmin, "Super Administrator with full system access"),
            (UserRole.Manager, "Manager with administrative privileges"),
            
            // Community Roles
            (UserRole.President, "Community President - Leadership role"),
            (UserRole.Secretary, "Community Secretary - Administrative role"),
            (UserRole.Treasurer, "Community Treasurer - Financial management role"),
            
            // Resident Roles
            (UserRole.ResidentOwner, "Resident Owner of the flat"),
            (UserRole.Tenant, "Tenant renting the flat"),
            
            // Staff Roles
            (UserRole.Security, "Security personnel"),
            (UserRole.Plumber, "Plumber staff"),
            (UserRole.Electrician, "Electrician staff"),
            (UserRole.Carpenter, "Carpenter staff"),
            (UserRole.Sweeper, "Cleaning staff"),
            (UserRole.Gardener, "Gardening staff"),
            (UserRole.MaintenanceStaff, "General maintenance staff")
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