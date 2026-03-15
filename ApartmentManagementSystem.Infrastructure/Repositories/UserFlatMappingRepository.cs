using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class UserFlatMappingRepository : GenericRepository<UserFlatMapping>, IUserFlatMappingRepository
    {
        public UserFlatMappingRepository(AppDbContext context) : base(context) { }

        public async Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId)
            => await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.Apartment)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.OwnerUser)
                .Where(ufm => ufm.UserId == userId)
                .OrderByDescending(ufm => ufm.FromDate)
                .ToListAsync();

        public async Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId)
            => await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                .Where(ufm => ufm.FlatId == flatId)
                .OrderByDescending(ufm => ufm.FromDate)
                .ToListAsync();

        public async Task<UserFlatMapping?> GetActiveMappingByUserIdAsync(Guid userId)
            => await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.Apartment)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.OwnerUser)
                .FirstOrDefaultAsync(ufm => ufm.UserId == userId && ufm.IsActive);

        public async Task<UserFlatMapping?> GetActiveMappingByFlatIdAsync(Guid flatId)
            => await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                .FirstOrDefaultAsync(ufm => ufm.FlatId == flatId && ufm.IsActive);
    }
}






/*using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository responsible for handling data access operations
    /// related to User-Flat mappings.
    /// 
    /// Handles:
    /// - Retrieving mappings by user or flat
    /// - Retrieving active mappings
    /// - Creating and updating mapping records
    /// 
    /// Note:
    /// Add and Update methods do not automatically save changes.
    /// Caller must invoke SaveChangesAsync().
    /// </summary>
    public class UserFlatMappingRepository : IUserFlatMappingRepository
    {
        /// <summary>
        /// Database context used for persistence operations.
        /// </summary>
        private readonly AppDbContext DBContext;

        /// <summary>
        /// Constructor for injecting the application database context.
        /// </summary>
        /// <param name="context">
        /// Application database context.
        /// </param>
        public UserFlatMappingRepository(AppDbContext context)
        {
            DBContext = context;
        }

        /// <summary>
        /// Retrieves a user-flat mapping by its unique identifier,
        /// including related user and flat information.
        /// </summary>
        /// <param name="id">
        /// Unique identifier of the mapping.
        /// </param>
        /// <returns>
        /// UserFlatMapping entity if found; otherwise null.
        /// </returns>
        public async Task<UserFlatMapping?> GetByIdAsync(Guid id)
        {
            return await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.Apartment)
                .FirstOrDefaultAsync(ufm => ufm.Id == id);
        }

        /// <summary>
        /// Retrieves all mappings for a specific user,
        /// ordered by most recent mapping first.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the user.
        /// </param>
        /// <returns>
        /// List of user-flat mappings.
        /// </returns>
        public async Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId)
        {
            return await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.Apartment)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.OwnerUser)
                .Where(ufm => ufm.UserId == userId)
                .OrderByDescending(ufm => ufm.FromDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all mappings for a specific flat,
        /// ordered by most recent mapping first.
        /// </summary>
        /// <param name="flatId">
        /// Unique identifier of the flat.
        /// </param>
        /// <returns>
        /// List of user-flat mappings.
        /// </returns>
        public async Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId)
        {
            return await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                .Where(ufm => ufm.FlatId == flatId)
                .OrderByDescending(ufm => ufm.FromDate)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the active mapping for a specific user.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the user.
        /// </param>
        /// <returns>
        /// Active mapping if exists; otherwise null.
        /// </returns>
        public async Task<UserFlatMapping?> GetActiveMappingByUserIdAsync(Guid userId)
        {
            return await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.Apartment)
                .Include(ufm => ufm.Flat)
                    .ThenInclude(f => f.OwnerUser)
                .FirstOrDefaultAsync(ufm => ufm.UserId == userId && ufm.IsActive);
        }

        /// <summary>
        /// Retrieves the active mapping for a specific flat.
        /// </summary>
        /// <param name="flatId">
        /// Unique identifier of the flat.
        /// </param>
        /// <returns>
        /// Active mapping if exists; otherwise null.
        /// </returns>
        public async Task<UserFlatMapping?> GetActiveMappingByFlatIdAsync(Guid flatId)
        {
            return await DBContext.UserFlatMappings
                .Include(ufm => ufm.User)
                .Include(ufm => ufm.Flat)
                .FirstOrDefaultAsync(ufm => ufm.FlatId == flatId && ufm.IsActive);
        }

        /// <summary>
        /// Adds a new user-flat mapping to the database.
        /// 
        /// Note: Changes are not saved immediately.
        /// </summary>
        /// <param name="mapping">
        /// UserFlatMapping entity to be added.
        /// </param>
        public async Task AddAsync(UserFlatMapping mapping)
        {
            await DBContext.UserFlatMappings.AddAsync(mapping);
        }

        /// <summary>
        /// Updates an existing user-flat mapping.
        /// 
        /// Note: Changes are not saved immediately.
        /// </summary>
        /// <param name="mapping">
        /// Updated UserFlatMapping entity.
        /// </param>
        public async Task UpdateAsync(UserFlatMapping mapping)
        {
            DBContext.UserFlatMappings.Update(mapping);
        }

        /// <summary>
        /// Persists all pending changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await DBContext.SaveChangesAsync();
        }
    }
}
*/



































