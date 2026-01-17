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
                    .ThenInclude(ufm => ufm.User)
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
/*
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Flats.CountAsync();
        }*/
        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Flats
                .AsNoTracking()
                .CountAsync();
        }


        /*    public async Task<int> GetOccupiedCountAsync()
            {
                return await _context.Flats
                    .Where(f => f.UserFlatMappings.Any(m => m.IsActive))
                    .CountAsync();
            }*/
        public async Task<int> GetOccupiedCountAsync()
        {
            return await _context.UserFlatMappings
                .AsNoTracking()
                .Where(m => m.IsActive)
                .Select(m => m.FlatId)
                .Distinct()
                .CountAsync();
        }


        public async Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerUserId)
        {
            return await _context.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                    .ThenInclude(ufm => ufm.User)
                .Where(f => f.OwnerUserId == ownerUserId)
                .ToListAsync();
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
                .Include(f => f.TenantUser
                )
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Flat>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Flats
                .Include(f => f.Apartment)
                .Where(f => f.OwnerUserId == userId || f.TenantUserId == userId)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Flats.CountAsync();
        }

        public async Task<int> GetOccupiedCountAsync()
        {
            return await _context.Flats
                .Where(f => f.OwnerUserId != null || f.TenantUserId != null)
                .CountAsync();
        }
        public async Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerUserId)
        {
            return await _context.Flats
                .Include(f => f.Apartment)
                .Include(f => f.OwnerUser)
                .Include(f => f.UserFlatMappings)
                    .ThenInclude(ufm => ufm.User)
                .Where(f => f.OwnerUserId == ownerUserId)
                .ToListAsync();
        }
    }
}
*/