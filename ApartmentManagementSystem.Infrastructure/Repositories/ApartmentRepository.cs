using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

/*

// Infrastructure/Persistence/Repositories/ApartmentRepository.cs
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    // Infrastructure/Persistence/Repositories/ApartmentRepository.cs
        public class ApartmentRepository : IApartmentRepository
        {
            private readonly AppDbContext _context;

            public ApartmentRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<Apartment?> GetByIdAsync(Guid id)
            {
                return await _context.Apartments.FindAsync(id);
            }

            public async Task<Apartment?> GetByIdWithFloorsAndFlatsAsync(Guid id)
            {
                return await _context.Apartments
                    .Include(a => a.Floors)
                        .ThenInclude(f => f.Flats)
                            .ThenInclude(flat => flat.UserFlatMappings!)
                                .ThenInclude(ufm => ufm.User)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }

            public async Task<Apartment?> GetByIdWithFullDetailsAsync(Guid id)
            {
                return await _context.Apartments
                    .Include(a => a.Floors)
                    .Include(a => a.Flats)
                    .Include(a => a.Managers.Where(m => m.IsActive))
                        .ThenInclude(m => m.User)
                    .Include(a => a.CommunityMembers.Where(cm => cm.IsActive))
                        .ThenInclude(cm => cm.User)
                            .ThenInclude(u => u.UserFlatMappings!)
                                .ThenInclude(ufm => ufm.Flat)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }

            public async Task<List<Apartment>> GetAllWithDetailsAsync()
            {
                return await _context.Apartments
                    .Include(a => a.Flats)
                    .Include(a => a.Managers.Where(m => m.IsActive))
                    .Where(a => a.IsActive)
                    .ToListAsync();
            }

            public async Task<List<Apartment>> GetAllAsync()
            {
                return await _context.Apartments
                    .Where(a => a.IsActive)
                    .ToListAsync();
            }

            public async Task<int> GetTotalCountAsync()
            {
                return await _context.Apartments.CountAsync(a => a.IsActive);
            }

            public async Task<ApartmentManager?> GetActiveManagerAsync(Guid apartmentId)
            {
                return await _context.Set<ApartmentManager>()
                    .Include(m => m.User)
                    .FirstOrDefaultAsync(m => m.ApartmentId == apartmentId && m.IsActive);
            }

            public async Task AddAsync(Apartment apartment)
            {
                await _context.Apartments.AddAsync(apartment);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateAsync(Apartment apartment)
            {
                _context.Apartments.Update(apartment);
                await _context.SaveChangesAsync();
            }

            public async Task AddManagerAsync(ApartmentManager manager)
            {
                await _context.Set<ApartmentManager>().AddAsync(manager);
                await _context.SaveChangesAsync();
            }

            public async Task UpdateManagerAsync(ApartmentManager manager)
            {
                _context.Set<ApartmentManager>().Update(manager);
                await _context.SaveChangesAsync();
            }

            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }
        }
    }
*/


namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly AppDbContext DBContext;

        public ApartmentRepository(AppDbContext context)
        {
            DBContext = context;
        }

        public async Task<Apartment?> GetByIdAsync(Guid id)
        {
            return await DBContext.Apartments.FindAsync(id);
        }

        public async Task<Apartment?> GetByIdWithFloorsAndFlatsAsync(Guid id)
        {
            return await DBContext.Apartments
                .Include(a => a.Floors)
                    .ThenInclude(f => f.Flats)
                        .ThenInclude(flat => flat.UserFlatMappings!)
                            .ThenInclude(ufm => ufm.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Apartment?> GetByIdWithFullDetailsAsync(Guid id)
        {
            return await DBContext.Apartments
                .Include(a => a.Floors)
                .Include(a => a.Flats)
                .Include(a => a.Managers.Where(m => m.IsActive))
                    .ThenInclude(m => m.User)
                .Include(a => a.CommunityMembers.Where(cm => cm.IsActive))
                    .ThenInclude(cm => cm.User)
                        .ThenInclude(u => u.UserFlatMappings!)
                            .ThenInclude(ufm => ufm.Flat)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Apartment>> GetAllWithDetailsAsync()
        {
            return await DBContext.Apartments
                .Include(a => a.Flats)
                .Include(a => a.Managers.Where(m => m.IsActive))
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        public async Task<List<Apartment>> GetAllAsync()
        {
            return await DBContext.Apartments
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await DBContext.Apartments.CountAsync(a => a.IsActive);
        }

        public async Task<ApartmentManager?> GetActiveManagerAsync(Guid apartmentId)
        {
            return await DBContext.Set<ApartmentManager>()
                .Include(m => m.User)
                .Include(m => m.Apartment)
                .FirstOrDefaultAsync(m => m.ApartmentId == apartmentId && m.IsActive);
        }

        // ⭐ NEW: Get manager by user ID
        public async Task<ApartmentManager?> GetActiveManagerByUserIdAsync(Guid userId)
        {
            return await DBContext.Set<ApartmentManager>()
                .Include(m => m.Apartment)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);
        }

        public async Task AddAsync(Apartment apartment)
        {
            await DBContext.Apartments.AddAsync(apartment);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Apartment apartment)
        {
            DBContext.Apartments.Update(apartment);
            await DBContext.SaveChangesAsync();
        }

        public async Task AddManagerAsync(ApartmentManager manager)
        {
            await DBContext.Set<ApartmentManager>().AddAsync(manager);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateManagerAsync(ApartmentManager manager)
        {
            DBContext.Set<ApartmentManager>().Update(manager);
            await DBContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await DBContext.SaveChangesAsync();
        }
    }
}