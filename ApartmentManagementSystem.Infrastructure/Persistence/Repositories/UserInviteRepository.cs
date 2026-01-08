using ApartmentManagementSystem.Domain.Domain.Entities;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Repositories
{
    public class UserInviteRepository : IUserInviteRepository
    {
        private readonly AppDbContext _context;

        public UserInviteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserInvite invite)
        {
            await _context.UserInvites.AddAsync(invite);
            await _context.SaveChangesAsync();
        }

        public async Task<UserInvite?> GetValidInviteAsync(string email)
        {
            return await _context.UserInvites
                .FirstOrDefaultAsync(x =>
                    x.Email == email &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task UpdateAsync(UserInvite invite)
        {
            _context.UserInvites.Update(invite);
            await _context.SaveChangesAsync();
        }
    }
}