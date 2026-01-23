using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Application.Interfaces.Repositories;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class UserFlatMappingRepository : IUserFlatMappingRepository
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














/*using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

using ApartmentManagementSystem.Application.Interfaces.Repositories;


    namespace ApartmentManagementSystem.Infrastructure.Repositories
    {
        public class UserFlatMappingRepository : IUserFlatMappingRepository
        {
            private readonly AppDbContext Dbcontext;

            public UserFlatMappingRepository(AppDbContext context)
            {
                Dbcontext = context;
            }

            public async Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId)
            {
                return await Dbcontext.UserFlatMappings
                    .Include(m => m.Flat)
                        .ThenInclude(f => f.Apartment)
                    .Where(m => m.UserId == userId && m.IsActive)
                    .ToListAsync();
            }

            public async Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId)
            {
                return await Dbcontext.UserFlatMappings
                    .Include(m => m.User)
                        .ThenInclude(u => u.Role)
                    .Where(m => m.FlatId == flatId && m.IsActive)
                    .ToListAsync();
            }

            public async Task AddAsync(UserFlatMapping mapping)
            {
                await Dbcontext.UserFlatMappings.AddAsync(mapping);
            }
        public async Task<UserFlatMapping> CreateAsync(UserFlatMapping mapping)
        {
            await AddAsync(mapping);
            await SaveChangesAsync();
            return mapping;
        }
        public async Task SaveChangesAsync()
            {
                await Dbcontext.SaveChangesAsync();
            }
        }
    }
*/