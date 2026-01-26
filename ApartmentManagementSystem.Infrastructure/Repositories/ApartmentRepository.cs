using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly AppDbContext DBcontext;

        public ApartmentRepository(AppDbContext context)
        {
            DBcontext = context;
        }

        public async Task<List<Apartment>> GetAllAsync()
            => await DBcontext.Apartments.ToListAsync();

        public async Task<Apartment?> GetByIdAsync(Guid id)
            => await DBcontext.Apartments.FindAsync(id);

        public async Task<int> GetTotalCountAsync()
            => await DBcontext.Apartments.CountAsync();

        public async Task AddAsync(Apartment apartment)
            => await DBcontext.Apartments.AddAsync(apartment);

        public async Task SaveChangesAsync()
            => await DBcontext.SaveChangesAsync();
    }
}

*/
  /*  public class ApartmentRepository : IApartmentRepository
    {
        private readonly AppDbContext _context;

        public ApartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Apartment?> GetByIdAsync(Guid id)
        {
            return await _context.Apartments
                .Include(a => a.Flats)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Apartment>> GetAllAsync()
        {
            return await _context.Apartments
                .Include(a => a.Flats)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Apartments.CountAsync();
        }
    }
}
  */

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
