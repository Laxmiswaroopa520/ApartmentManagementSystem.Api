/*using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FlatRepository : IFlatRepository
    {
        private readonly AppDbContext DBcontext;

        public FlatRepository(AppDbContext context)
        {
            DBcontext = context;
        }

        public async Task<Flat?> GetByIdAsync(Guid id)
        {
            return await DBcontext.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Flat>> GetByUserIdAsync(Guid userId)
        {
            return await DBcontext.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                .Where(f =>
                    f.OwnerUserId == userId ||
                    f.UserFlatMappings.Any(m => m.UserId == userId && m.IsActive))
                .ToListAsync();
        }
        public async Task<List<Floor>> GetAllFloorsAsync()
        {
            return await DBcontext.Floors.ToListAsync();
        }

        //  OWNER DASHBOARD METHOD
        public async Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerUserId)
        {
            return await DBcontext.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                    .ThenInclude(m => m.User)
                .Where(f => f.OwnerUserId == ownerUserId)
                .ToListAsync();
        }

        // ONBOARDING – AVAILABLE FLATS BY FLOOR
        public async Task<List<Flat>> GetAvailableFlatsByFloorAsync(Guid floorId)
        {
            return await DBcontext.Flats
                .AsNoTracking()
                .Where(f => f.FloorId == floorId && f.OwnerUserId == null && f.IsActive)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await DBcontext.Flats
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<int> GetOccupiedCountAsync()
        {
            return await DBcontext.Flats
                .AsNoTracking()
                .CountAsync(f => f.OwnerUserId != null);
        }

        /* public async Task UpdateAsync(Flat flat)
         {
             _context.Flats.Update(flat);
             await Task.CompletedTask;
         }--------
        public Task UpdateAsync(Flat flat)
        {
            DBcontext.Flats.Update(flat);
            return Task.CompletedTask;
        }

        public async Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId)
        {
            return await DBcontext.Flats
                .AsNoTracking()
                .Where(f => f.FloorId == floorId && f.OwnerUserId == null && f.IsActive)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }
       public async Task<Flat?> GetFlatByResidentIdAsync(Guid userId)
        {
            //  Find flat by checking Users who have this FlatId
            var user = await DBcontext.Users
                .Include(u => u.Flat)
                .ThenInclude(f => f.Apartment)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Flat;
        }
        /*  public async Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId)
          {
              return await _dbContext.Flats
                  .Where(f => f.FloorId == floorId && f.IsVacant)
                  .ToListAsync();
          }--------
        public async Task SaveChangesAsync()
        {
            await DBcontext.SaveChangesAsync();
        }
    }
}
*/
// Infrastructure/Persistence/Repositories/FlatRepository.cs
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Infrastructure.Persistence;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FlatRepository : IFlatRepository
    {
        private readonly AppDbContext _context;

        public FlatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Flat?> GetByIdAsync(Guid id)
        {
            return await _context.Flats
                .Include(f => f.Apartment)
                .Include(f => f.Floor)
                .Include(f => f.OwnerUser)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Flat>> GetByFloorIdAsync(Guid floorId)
        {
            return await _context.Flats
                .Where(f => f.FloorId == floorId)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        public async Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId)
        {
            return await _context.Flats
                .Where(f => f.FloorId == floorId && !f.IsOccupied && f.IsActive)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        public async Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Flats
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
            return await _context.Floors
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Flats.CountAsync(f => f.IsActive);
        }

        public async Task<int> GetOccupiedCountAsync()
        {
            return await _context.Flats.CountAsync(f => f.IsActive && f.IsOccupied);
        }

        public async Task AddAsync(Flat flat)
        {
            await _context.Flats.AddAsync(flat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Flat flat)
        {
            _context.Flats.Update(flat);
            // Don't save here - let SaveChangesAsync handle it
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}





/*

using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FlatRepository : IFlatRepository
    {
        private readonly AppDbContext _context;

        public FlatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Flat?> GetByIdAsync(Guid id)
        {
            return await _context.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Flat>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                .Where(f =>
                    f.OwnerUserId == userId ||
                    f.UserFlatMappings.Any(m => m.UserId == userId && m.IsActive))
                .ToListAsync();
        }

        // ✅ OWNER DASHBOARD METHOD
        public async Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerUserId)
        {
            return await _context.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                    .ThenInclude(m => m.User)
                .Where(f => f.OwnerUserId == ownerUserId)
                .ToListAsync();
        }

        // ✅ ONBOARDING – AVAILABLE FLATS BY FLOOR
        public async Task<List<Flat>> GetAvailableFlatsByFloorAsync(Guid floorId)
        {
            return await _context.Flats
                .AsNoTracking()
                .Where(f => f.FloorId == floorId && f.OwnerUserId == null && f.IsActive)
                .OrderBy(f => f.FlatNumber)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Flats
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<int> GetOccupiedCountAsync()
        {
            return await _context.Flats
                .AsNoTracking()
                .CountAsync(f => f.OwnerUserId != null);
        }

        public async Task UpdateAsync(Flat flat)
        {
            _context.Flats.Update(flat);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
*/
