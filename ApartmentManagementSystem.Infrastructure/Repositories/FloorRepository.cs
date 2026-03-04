using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Repository responsible for handling data access operations
    /// related to Floor entities.
    /// 
    /// Handles:
    /// - Retrieving floors
    /// - Filtering floors by apartment
    /// - Creating, updating, and deleting floors
    /// </summary>
    public class FloorRepository : IFloorRepository
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
        public FloorRepository(AppDbContext context)
        {
            DBContext = context;
        }

        /// <summary>
        /// Retrieves a floor by its unique identifier including
        /// related apartment and flats.
        /// </summary>
        /// <param name="id">
        /// Unique identifier of the floor.
        /// </param>
        /// <returns>
        /// Floor entity if found; otherwise null.
        /// </returns>
        public async Task<Floor?> GetByIdAsync(Guid id)
        {
            return await DBContext.Floors
                .Include(f => f.Apartment)
                .Include(f => f.Flats)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        /// <summary>
        /// Retrieves all floors ordered by floor number,
        /// including related apartment information.
        /// </summary>
        /// <returns>
        /// List of floors.
        /// </returns>
        public async Task<List<Floor>> GetAllAsync()
        {
            return await DBContext.Floors
                .Include(f => f.Apartment)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all floors belonging to a specific apartment.
        /// </summary>
        /// <param name="apartmentId">
        /// Unique identifier of the apartment.
        /// </param>
        /// <returns>
        /// List of floors associated with the apartment.
        /// </returns>
        public async Task<List<Floor>> GetByApartmentIdAsync(Guid apartmentId)
        {
            return await DBContext.Floors
                .Include(f => f.Apartment)
                .Where(f => f.ApartmentId == apartmentId)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new floor to the database.
        /// </summary>
        /// <param name="floor">
        /// Floor entity to be added.
        /// </param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task AddAsync(Floor floor)
        {
            await DBContext.Floors.AddAsync(floor);
            await DBContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing floor record.
        /// </summary>
        /// <param name="floor">
        /// Floor entity containing updated values.
        /// </param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task UpdateAsync(Floor floor)
        {
            DBContext.Floors.Update(floor);
            await DBContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an existing floor from the database.
        /// </summary>
        /// <param name="floor">
        /// Floor entity to be removed.
        /// </param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task DeleteAsync(Floor floor)
        {
            DBContext.Floors.Remove(floor);
            await DBContext.SaveChangesAsync();
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








