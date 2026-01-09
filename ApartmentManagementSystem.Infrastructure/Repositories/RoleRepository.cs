using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Infrastructure.Persistence;
    using global::ApartmentManagementSystem.Application.Interfaces.Repositories;
    using global::ApartmentManagementSystem.Domain.Entities;
    using global::ApartmentManagementSystem.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

   // namespace ApartmentManagementSystem.Infrastructure.Repositories;

    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _db;
        public RoleRepository(AppDbContext db) => _db = db;

        public async Task<Role> GetByIdAsync(Guid id) =>
            await _db.Roles.FirstAsync(x => x.Id == id);
    }
}