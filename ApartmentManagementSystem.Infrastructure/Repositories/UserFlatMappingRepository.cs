using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Application.Interfaces.Repositories;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{

    // Infrastructure/Persistence/Repositories/UserFlatMappingRepository.cs
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
  public class UserFlatMappingRepository : IUserFlatMappingRepository
        {
            private readonly AppDbContext _context;

            public UserFlatMappingRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<UserFlatMapping?> GetByIdAsync(Guid id)
            {
                return await _context.UserFlatMappings
                    .Include(ufm => ufm.User)
                    .Include(ufm => ufm.Flat)
                        .ThenInclude(f => f.Apartment)
                    .FirstOrDefaultAsync(ufm => ufm.Id == id);
            }

            public async Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId)
            {
                return await _context.UserFlatMappings
                    .Include(ufm => ufm.User)
                    .Include(ufm => ufm.Flat)
                        .ThenInclude(f => f.Apartment)
                    .Include(ufm => ufm.Flat)
                        .ThenInclude(f => f.OwnerUser)
                    .Where(ufm => ufm.UserId == userId)
                    .OrderByDescending(ufm => ufm.FromDate)
                    .ToListAsync();
            }

            public async Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId)
            {
                return await _context.UserFlatMappings
                    .Include(ufm => ufm.User)
                    .Include(ufm => ufm.Flat)
                    .Where(ufm => ufm.FlatId == flatId)
                    .OrderByDescending(ufm => ufm.FromDate)
                    .ToListAsync();
            }

            public async Task<UserFlatMapping?> GetActiveMappingByUserIdAsync(Guid userId)
            {
                return await _context.UserFlatMappings
                    .Include(ufm => ufm.User)
                    .Include(ufm => ufm.Flat)
                        .ThenInclude(f => f.Apartment)
                    .Include(ufm => ufm.Flat)
                        .ThenInclude(f => f.OwnerUser)
                    .FirstOrDefaultAsync(ufm => ufm.UserId == userId && ufm.IsActive);
            }

            public async Task<UserFlatMapping?> GetActiveMappingByFlatIdAsync(Guid flatId)
            {
                return await _context.UserFlatMappings
                    .Include(ufm => ufm.User)
                    .Include(ufm => ufm.Flat)
                    .FirstOrDefaultAsync(ufm => ufm.FlatId == flatId && ufm.IsActive);
            }

            public async Task AddAsync(UserFlatMapping mapping)
            {
                await _context.UserFlatMappings.AddAsync(mapping);
                // Don't save here
            }

            public async Task UpdateAsync(UserFlatMapping mapping)
            {
                _context.UserFlatMappings.Update(mapping);
                // Don't save here
            }

            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }
        }
    }



























    /*public class UserFlatMappingRepository : IUserFlatMappingRepository
    {
        private readonly AppDbContext DbContext;

        public UserFlatMappingRepository(AppDbContext context)
        {
            DbContext = context;
        }

        public async Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId)
        {
            return await DbContext.UserFlatMappings
                .Include(m => m.Flat)
                    .ThenInclude(f => f.Apartment)
                .Where(m => m.UserId == userId && m.IsActive)
                .ToListAsync();
        }

        public async Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId)
        {
            return await DbContext.UserFlatMappings
                .Include(m => m.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .Where(m => m.FlatId == flatId && m.IsActive)
                .ToListAsync();
        }

        public async Task AddAsync(UserFlatMapping mapping)
        {
            await DbContext.UserFlatMappings.AddAsync(mapping);
        }

        public async Task<UserFlatMapping> CreateAsync(UserFlatMapping mapping)
        {
            await AddAsync(mapping);
            await SaveChangesAsync();
            return mapping;
        }

        public async Task SaveChangesAsync()
        {
            await DbContext.SaveChangesAsync();
        }
    }
}

*/












