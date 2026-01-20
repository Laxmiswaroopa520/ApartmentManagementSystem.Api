using ApartmentManagementSystem.Application.Interfaces.Repositories;
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
