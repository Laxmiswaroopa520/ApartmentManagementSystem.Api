using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext DBContext;

    public UserRepository(AppDbContext context)
    {
        DBContext = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await DBContext.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByUsernameWithRolesAsync(string username)
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

  /*  public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.PrimaryPhone == phone);
    }*/

    public async Task AddAsync(User user)
    {
        await DBContext.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        DBContext.Users.Update(user);
        await DBContext.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
        => DBContext.SaveChangesAsync();

    public async Task<bool> PhoneExistsAsync(string phone)
        => await DBContext.Users.AnyAsync(u => u.PrimaryPhone == phone);

    public async Task<bool> UsernameExistsAsync(string username)
        => await DBContext.Users.AnyAsync(u => u.Username == username);

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await DBContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetPendingResidentsAsync()
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.Status == ResidentStatus.PendingFlatAllocation)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }
    //added this part for flat dropdown for manager
    public async Task<List<User>> GetUsersByRoleWithFlatsAsync(string roleName)
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserFlatMappings!)
                .ThenInclude(ufm => ufm.Flat)
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
            .ToListAsync();
    }

    // This method is for assigning manager..
    /* public async Task<List<User>> GetUsersByRoleAsync(string roleName)
     {
         return await DBContext.Users
             .Include(u => u.UserRoles)
                 .ThenInclude(ur => ur.Role)
             .Include(u => u.UserFlatMappings)
                 .ThenInclude(ufm => ufm.Flat)
             .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
             .OrderBy(u => u.FullName)
             .AsNoTracking()
             .ToListAsync();
     }*/


    //new method for manager

    /// <summary>
    /// Creates a new User and assigns them the given role.
    /// Used for external managers who don't already exist in the system.
    /// </summary>
    /*  public async Task CreateExternalManagerUserAsync(User user, string roleName)
      {
          // ⭐ FIX: User entity has NO "CreatedBy" property.
          // It only has: CreatedAt, UpdatedAt, UpdatedBy.
          // So we only set what actually exists on the entity.
          // The caller (ManagerService) already sets:
          //   Id, FullName, PrimaryPhone, Email, IsActive, CreatedAt
          // Email is already string? on User, so null is fine.

          await DBContext.Users.AddAsync(user);

          var role = await DBContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
          if (role == null)
              throw new Exception($"Role '{roleName}' not found in the database.");

          var userRole = new UserRole
          {
              Id = Guid.NewGuid(),

              UserId = user.Id,
              RoleId = role.Id,
              AssignedAt = DateTime.UtcNow
          };

          await DBContext.Set<UserRole>().AddAsync(userRole);
          await DBContext.SaveChangesAsync();
      }
    */
    //METHOD 1: Get user by phone number
    // Add to your existing UserRepository class

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.PrimaryPhone == phone);
    }
  /*  public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.PrimaryPhone == phone);
    }
  */
    // METHOD 2: Create external manager user (with Manager role)
    public async Task CreateExternalManagerUserAsync(User user, string roleName)
    {
        // Add user to database
        await DBContext.Users.AddAsync(user);
        await DBContext.SaveChangesAsync();

        // Get Manager role
        var role = await DBContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
            throw new Exception($"Role '{roleName}' not found in the system");

        // Assign Manager role to this user
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
            AssignedAt = DateTime.UtcNow
        };

        await DBContext.Set<UserRole>().AddAsync(userRole);
        await DBContext.SaveChangesAsync();
    }

    // METHOD 3: Add role to existing user
    public async Task AddRoleToUserAsync(Guid userId, string roleName)
    {
        // Check if user already has this role
        var existingRole = await DBContext.Set<UserRole>()
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);

        if (existingRole != null)
        {
            Console.WriteLine($"User {userId} already has {roleName} role");
            return; // Already has this role
        }

        // Get the role
        var role = await DBContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
            throw new Exception($"Role '{roleName}' not found");

        // Assign the role
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = role.Id,
            AssignedAt = DateTime.UtcNow
        };

        await DBContext.Set<UserRole>().AddAsync(userRole);
        await DBContext.SaveChangesAsync();

        Console.WriteLine($"Successfully added {roleName} role to user {userId}");
    }

    //METHOD 4: Update GetUsersByRoleAsync to include UserFlatMappings
    public async Task<List<User>> GetUsersByRoleAsync(string roleName)
    {
        return await DBContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserFlatMappings!)  // ⭐ CRITICAL - must include
                .ThenInclude(ufm => ufm.Flat)
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName) && u.IsActive)
            .ToListAsync();
    }




























    /*
    //new menthod for manager 
    public async Task CreateExternalManagerUserAsync(User user, string roleName)
    {
        // 1) Save the user
        await DBContext.Users.AddAsync(user);

        // 2) Find the role record (e.g. "Manager")
        var role = await DBContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
            throw new Exception($"Role '{roleName}' not found in the database. Make sure it exists in the Roles table.");

        // 3) Create the UserRole link
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
            AssignedAt = DateTime.UtcNow
        };

        await DBContext.Set<UserRole>().AddAsync(userRole);
        await DBContext.SaveChangesAsync();
    }*/

}



























/*
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
   public class UserRepository : IUserRepository
    {
        private readonly AppDbContext DBContext;

        public UserRepository(AppDbContext db) => DBContext = db;

        // YOUR EXISTING METHODS
        public Task<User?> GetByUsernameAsync(string username) =>
            DBContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == username);

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await DBContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<User?> GetByPhoneAsync(string phone) =>
            DBContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.PrimaryPhone == phone);

        public async Task AddAsync(User user) => await DBContext.Users.AddAsync(user);

        public Task SaveChangesAsync() => DBContext.SaveChangesAsync();

        public async Task<List<User>> GetPendingResidentsAsync()
        {
            return await DBContext.Users
                .Include(u => u.Role)
                .Where(u => u.Status == ResidentStatus.PendingFlatAllocation)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
        }

        // ADDED: Missing methods
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await DBContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await DBContext.Users.AnyAsync(u => u.PrimaryPhone == phone);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await DBContext.Users.AnyAsync(u => u.Username == username);
        }
        //new method added here..
        public async Task<User?> GetByUsernameWithRolesAsync(string username)
        {
            return await DBContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task UpdateAsync(User user)
        {
            DBContext.Users.Update(user);
            await DBContext.SaveChangesAsync();
        }
    }
}
*/