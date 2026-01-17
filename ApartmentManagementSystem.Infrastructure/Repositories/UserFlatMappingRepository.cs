using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

using ApartmentManagementSystem.Application.Interfaces.Repositories;


    namespace ApartmentManagementSystem.Infrastructure.Repositories
    {
        public class UserFlatMappingRepository : IUserFlatMappingRepository
        {
            private readonly AppDbContext _context;

            public UserFlatMappingRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId)
            {
                return await _context.UserFlatMappings
                    .Include(m => m.Flat)
                        .ThenInclude(f => f.Apartment)
                    .Where(m => m.UserId == userId && m.IsActive)
                    .ToListAsync();
            }

            public async Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId)
            {
                return await _context.UserFlatMappings
                    .Include(m => m.User)
                        .ThenInclude(u => u.Role)
                    .Where(m => m.FlatId == flatId && m.IsActive)
                    .ToListAsync();
            }

            public async Task AddAsync(UserFlatMapping mapping)
            {
                await _context.UserFlatMappings.AddAsync(mapping);
            }

            public async Task SaveChangesAsync()
            {
                await _context.SaveChangesAsync();
            }
        }
    }
