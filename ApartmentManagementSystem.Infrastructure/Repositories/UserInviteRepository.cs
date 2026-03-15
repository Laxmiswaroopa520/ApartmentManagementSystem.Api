using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class UserInviteRepository : GenericRepository<UserInvite>, IUserInviteRepository
    {
        public UserInviteRepository(AppDbContext context) : base(context) { }

        public async Task<UserInvite?> GetByIdWithRoleAsync(Guid id)
            => await DBContext.UserInvites
                .Include(i => i.Role)
                .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<UserInvite?> GetByPhoneAsync(string phone)
            => await DBContext.UserInvites
                .Include(i => i.Role)
                .FirstOrDefaultAsync(i => i.PrimaryPhone == phone);

        // Override base GetAllAsync for ordered + included result
        public new async Task<List<UserInvite>> GetAllAsync()
            => await DBContext.UserInvites
                .Include(i => i.Role)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

        /// <summary>
        /// Mutates invite status in memory. Caller calls UoW.SaveChangesAsync().
        /// </summary>
        public async Task UpdateStatusAsync(Guid inviteId, string status)
        {
            var invite = await DBContext.UserInvites.FindAsync(inviteId);
            if (invite != null)
                invite.InviteStatus = status;
        }
    }
}










/*namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    public class UserInviteRepository : IUserInviteRepository
    {
        private readonly AppDbContext DBContext;

        public UserInviteRepository(AppDbContext db)
        {
            DBContext = db;
        }

        public async Task<UserInvite?> GetByIdAsync(Guid id)
        {
            return await DBContext.UserInvites
                .Include(i => i.Role)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<UserInvite?> GetByPhoneAsync(string phone)
        {
            return await DBContext.UserInvites
                .Include(i => i.Role)
                .FirstOrDefaultAsync(i => i.PrimaryPhone == phone);
        }

        public async Task<UserInvite> CreateAsync(UserInvite invite)
        {
            await DBContext.UserInvites.AddAsync(invite);
            await DBContext.SaveChangesAsync();
            return invite;
        }

        public async Task UpdateStatusAsync(Guid inviteId, string status)
        {
            var invite = await DBContext.UserInvites.FindAsync(inviteId);
            if (invite != null)
            {
                invite.InviteStatus = status;
                await DBContext.SaveChangesAsync();
            }
        }

        public async Task<List<UserInvite>> GetAllAsync()
        {
            return await DBContext.UserInvites
                .Include(i => i.Role)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await DBContext.SaveChangesAsync();
        }
    }
}
*/

















