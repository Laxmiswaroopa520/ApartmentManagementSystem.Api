using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByUsernameWithRolesAsync(string username)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.PrimaryPhone == phone);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();

    public async Task<bool> PhoneExistsAsync(string phone)
        => await _context.Users.AnyAsync(u => u.PrimaryPhone == phone);

    public async Task<bool> UsernameExistsAsync(string username)
        => await _context.Users.AnyAsync(u => u.Username == username);

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetPendingResidentsAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.Status == ResidentStatus.PendingFlatAllocation)
            .OrderBy(u => u.CreatedAt)
            .ToListAsync();
    }
}



























/*
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
   public class UserRepository : IUserRepository
    {
        private readonly AppDbContext DBContext;

        public UserRepository(AppDbContext db) => DBContext = db;

        // YOUR EXISTING METHODS
        public Task<User?> GetByUsernameAsync(string username) =>
            DBContext.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Username == username);

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await DBContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<User?> GetByPhoneAsync(string phone) =>
            DBContext.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.PrimaryPhone == phone);

        public async Task AddAsync(User user) => await DBContext.Users.AddAsync(user);

        public Task SaveChangesAsync() => DBContext.SaveChangesAsync();

        public async Task<List<User>> GetPendingResidentsAsync()
        {
            return await DBContext.Users
                .Include(u => u.Role)
                .Where(u => u.Status == ResidentStatus.PendingFlatAllocation)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();
        }

        // ADDED: Missing methods
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await DBContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await DBContext.Users.AnyAsync(u => u.PrimaryPhone == phone);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await DBContext.Users.AnyAsync(u => u.Username == username);
        }
        //new method added here..
        public async Task<User?> GetByUsernameWithRolesAsync(string username)
        {
            return await DBContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task UpdateAsync(User user)
        {
            DBContext.Users.Update(user);
            await DBContext.SaveChangesAsync();
        }
    }
}
*/