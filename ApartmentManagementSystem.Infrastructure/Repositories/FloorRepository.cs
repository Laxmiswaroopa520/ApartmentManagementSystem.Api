using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FloorRepository : IFloorRepository
    {
        private readonly AppDbContext DBContext;

        public FloorRepository(AppDbContext context)
        {
            DBContext = context;
        }

        public async Task<Floor?> GetByIdAsync(Guid id)
        {
            return await DBContext.Floors
                .Include(f => f.Apartment)
                .Include(f => f.Flats)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Floor>> GetAllAsync()
        {
            return await DBContext.Floors
                .Include(f => f.Apartment)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        // ⭐ NEW: Get floors by apartment
        public async Task<List<Floor>> GetByApartmentIdAsync(Guid apartmentId)
        {
            return await DBContext.Floors
                .Include(f => f.Apartment)
                .Where(f => f.ApartmentId == apartmentId)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public async Task AddAsync(Floor floor)
        {
            await DBContext.Floors.AddAsync(floor);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Floor floor)
        {
            DBContext.Floors.Update(floor);
            await DBContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Floor floor)
        {
            DBContext.Floors.Remove(floor);
            await DBContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await DBContext.SaveChangesAsync();
        }
    }
}














/*
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Infrastructure.Persistence;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FloorRepository : IFloorRepository
    {
        private readonly AppDbContext _context;

        public FloorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Floor?> GetByIdAsync(Guid id)
        {
            return await _context.Floors.FindAsync(id);
        }

        public async Task<List<Floor>> GetByApartmentIdAsync(Guid apartmentId)
        {
            return await _context.Floors
                .Where(f => f.ApartmentId == apartmentId)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public async Task AddAsync(Floor floor)
        {
            await _context.Floors.AddAsync(floor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Floor floor)
        {
            _context.Floors.Update(floor);
            await _context.SaveChangesAsync();
        }
    }
}
*/

