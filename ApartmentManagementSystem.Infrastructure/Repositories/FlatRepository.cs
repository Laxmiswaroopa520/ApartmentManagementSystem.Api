// Infrastructure/Persistence/Repositories/FlatRepository.cs
// Infrastructure/Persistence/Repositories/FlatRepository.cs

using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Infrastructure.Persistence;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository responsible for handling data access operations
    /// related to Flats and associated Floor information.
    /// 
    /// Handles:
    /// - Retrieving flats by various filters
    /// - Vacancy and occupancy statistics
    /// - Floor retrieval
    /// - Adding and updating flat records
    /// </summary>
    public class FlatRepository : IFlatRepository
    {
        /// <summary>
        /// Database context used for accessing persistence layer.
        /// </summary>
        private readonly AppDbContext DBContext;

        /// <summary>
        /// Constructor for injecting database context dependency.
        /// </summary>
        /// <param name="context">
        /// Application database context.
        /// </param>
        public FlatRepository(AppDbContext context)
        {
            DBContext = context;
        }

        /// <summary>
        /// Retrieves a flat by its unique identifier including related data.
        /// </summary>
        /// <param name="id">
        /// Unique identifier of the flat.
        /// </param>
        /// <returns>
        /// Flat entity if found; otherwise null.
        /// </returns>
        public async Task<Flat?> GetByIdAsync(Guid id)
        {
            return await DBContext.Flats
                .Include(f => f.Apartment)
                .Include(f => f.Floor)
                .Include(f => f.OwnerUser)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Retrieves all flats belonging to a specific floor.
        /// </summary>
        /// <param name="floorId">
        /// Unique identifier of the floor.
        /// </param>
        /// <returns>
        /// List of flats ordered by flat number.
        /// </returns>
        public async Task<List<Flat>> GetByFloorIdAsync(Guid floorId)
        {
            return await DBContext.Flats
                .Where(f => f.FloorId == floorId)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all vacant and active flats for a specific floor.
        /// </summary>
        /// <param name="floorId">
        /// Unique identifier of the floor.
        /// </param>
        /// <returns>
        /// List of vacant flats ordered by flat number.
        /// </returns>
        public async Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId)
        {
            return await DBContext.Flats
                .Where(f => f.FloorId == floorId && !f.IsOccupied && f.IsActive)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves flats owned by a specific owner including
        /// related apartment, floor, owner, and user mappings.
        /// </summary>
        /// <param name="ownerId">
        /// Unique identifier of the owner.
        /// </param>
        /// <returns>
        /// List of flats with associated mappings.
        /// </returns>
        public async Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerId)
        {
            return await DBContext.Flats
                .Include(f => f.Apartment)
                .Include(f => f.Floor)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings!)
                    .ThenInclude(ufm => ufm.User)
                .Where(f => f.OwnerUserId == ownerId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all floors ordered by floor number.
        /// </summary>
        /// <returns>
        /// List of floors.
        /// </returns>
        public async Task<List<Floor>> GetAllFloorsAsync()
        {
            return await DBContext.Floors
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves total count of active flats.
        /// </summary>
        /// <returns>
        /// Total number of active flats.
        /// </returns>
        public async Task<int> GetTotalCountAsync()
        {
            return await DBContext.Flats.CountAsync(f => f.IsActive);
        }

        /// <summary>
        /// Retrieves total count of occupied and active flats.
        /// </summary>
        /// <returns>
        /// Total number of occupied flats.
        /// </returns>
        public async Task<int> GetOccupiedCountAsync()
        {
            return await DBContext.Flats
                .CountAsync(f => f.IsActive && f.IsOccupied);
        }

        /// <summary>
        /// Adds a new flat record to the database.
        /// </summary>
        /// <param name="flat">
        /// Flat entity to be added.
        /// </param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task AddAsync(Flat flat)
        {
            await DBContext.Flats.AddAsync(flat);
            await DBContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing flat record.
        /// 
        /// Note: Changes are not saved immediately.
        /// Caller must invoke SaveChangesAsync().
        /// </summary>
        /// <param name="flat">
        /// Updated flat entity.
        /// </param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task UpdateAsync(Flat flat)
        {
            DBContext.Flats.Update(flat);
        }

        /// <summary>
        /// Persists all pending changes to the database.
        /// </summary>
        /// <returns>
        /// Task representing the save operation.
        /// </returns>
        public async Task SaveChangesAsync()
        {
            await DBContext.SaveChangesAsync();
        }
    }
}
























/*using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Infrastructure.Persistence;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FlatRepository : IFlatRepository
    {
        private readonly AppDbContext DBContext;

        public FlatRepository(AppDbContext context)
        {
            DBContext = context;
        }

        public async Task<Flat?> GetByIdAsync(Guid id)
        {
            return await DBContext.Flats
                .Include(f => f.Apartment)
                .Include(f => f.Floor)
                .Include(f => f.OwnerUser)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Flat>> GetByFloorIdAsync(Guid floorId)
        {
            return await DBContext.Flats
                .Where(f => f.FloorId == floorId)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        public async Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId)
        {
            return await DBContext.Flats
                .Where(f => f.FloorId == floorId && !f.IsOccupied && f.IsActive)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        public async Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerId)
        {
            return await DBContext.Flats
                .Include(f => f.Apartment)
                .Include(f => f.Floor)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings!)
                    .ThenInclude(ufm => ufm.User)
                .Where(f => f.OwnerUserId == ownerId)
                .ToListAsync();
        }

        public async Task<List<Floor>> GetAllFloorsAsync()
        {
            return await DBContext.Floors
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await DBContext.Flats.CountAsync(f => f.IsActive);
        }

        public async Task<int> GetOccupiedCountAsync()
        {
            return await DBContext.Flats.CountAsync(f => f.IsActive && f.IsOccupied);
        }

        public async Task AddAsync(Flat flat)
        {
            await DBContext.Flats.AddAsync(flat);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Flat flat)
        {
            DBContext.Flats.Update(flat);
            // Don't save here - let SaveChangesAsync handle it
        }

        public async Task SaveChangesAsync()
        {
            await DBContext.SaveChangesAsync();
        }
    }
}
*/


