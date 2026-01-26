/*using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FloorRepository : IFloorRepository
    {
        private readonly AppDbContext DBcontext;

        public FloorRepository(AppDbContext context)
        {
            DBcontext = context;
        }

        public async Task<List<Floor>> GetAllAsync()
        {
            return await DBcontext.Floors
                .AsNoTracking()
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }
    }
}
*/
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