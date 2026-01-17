using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly AppDbContext _context;

        public ApartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Apartment>> GetAllAsync()
            => await _context.Apartments.ToListAsync();

        public async Task<Apartment?> GetByIdAsync(Guid id)
            => await _context.Apartments.FindAsync(id);

        public async Task<int> GetTotalCountAsync()
            => await _context.Apartments.CountAsync();

        public async Task AddAsync(Apartment apartment)
            => await _context.Apartments.AddAsync(apartment);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}


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