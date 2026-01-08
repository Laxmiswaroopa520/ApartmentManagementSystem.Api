using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
//using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext DbContext;

        public UserRepository(AppDbContext db)
        {
            DbContext = db;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await DbContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task AddAsync(User user)
        {
            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();
        }
    }
}