
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

   // namespace ApartmentManagementSystem.Infrastructure.Repositories;

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) => _db = db;

        public Task<User?> GetByUsernameAsync(string username) =>
            _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == username);

        public Task<User?> GetByPhoneAsync(string phone) =>
            _db.Users.FirstOrDefaultAsync(x => x.PrimaryPhone == phone);

        public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}