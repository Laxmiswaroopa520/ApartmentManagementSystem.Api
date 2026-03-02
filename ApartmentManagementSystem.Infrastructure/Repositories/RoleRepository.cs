using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository responsible for handling data access operations
    /// related to Role entities.
    /// 
    /// Handles:
    /// - Retrieving roles by ID
    /// - Retrieving roles by name
    /// - Retrieving all roles
    /// 
    /// Uses AsNoTracking for read-only queries to improve performance.
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        /// <summary>
        /// Database context used for persistence operations.
        /// </summary>
        private readonly AppDbContext DBContext;

        /// <summary>
        /// Constructor for injecting the application database context.
        /// </summary>
        /// <param name="db">
        /// Application database context.
        /// </param>
        public RoleRepository(AppDbContext db)
        {
            DBContext = db;
        }

        /// <summary>
        /// Retrieves a role by its unique identifier.
        /// </summary>
        /// <param name="id">
        /// Unique identifier of the role.
        /// </param>
        /// <returns>
        /// Role entity if found; otherwise null.
        /// </returns>
        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await DBContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Retrieves all roles ordered alphabetically by name.
        /// </summary>
        /// <returns>
        /// List of roles.
        /// </returns>
        public async Task<List<Role>> GetAllAsync()
        {
            return await DBContext.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a role by its name.
        /// </summary>
        /// <param name="name">
        /// Name of the role.
        /// </param>
        /// <returns>
        /// Role entity if found; otherwise null.
        /// </returns>
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await DBContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
















/*using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext DBContext;

        public RoleRepository(AppDbContext db)
        {
            DBContext = db;
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await DBContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await DBContext.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await DBContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
*/