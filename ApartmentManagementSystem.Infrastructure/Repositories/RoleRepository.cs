using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext DBContext;

        public RoleRepository(AppDbContext db)
        {
            DBContext = db;
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await DBContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await DBContext.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await DBContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
