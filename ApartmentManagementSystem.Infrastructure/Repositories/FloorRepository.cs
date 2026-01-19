using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities.ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class FloorRepository : IFloorRepository
    {
        private readonly AppDbContext _context;

        public FloorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Floor>> GetAllAsync()
        {
            return await _context.Floors
                .AsNoTracking()
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }
    }
}
