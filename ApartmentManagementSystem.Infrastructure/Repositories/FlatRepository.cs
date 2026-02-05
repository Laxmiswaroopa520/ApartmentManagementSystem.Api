// Infrastructure/Persistence/Repositories/FlatRepository.cs
using ApartmentManagementSystem.Application.Interfaces.Repositories;
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



