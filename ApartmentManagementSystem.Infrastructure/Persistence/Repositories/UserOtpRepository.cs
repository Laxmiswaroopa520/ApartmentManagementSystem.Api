using ApartmentManagementSystem.Domain.Domain.Entities;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ApartmentManagementSystem.Infrastructure.Persistence.Repositories
{
    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly AppDbContext _context;

        public UserOtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserOtp otp)
        {
            await _context.UserOtps.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<UserOtp?> GetValidOtpAsync(string email, string otp)
        {
            return await _context.UserOtps.FirstOrDefaultAsync(x =>
                x.Email == email &&
                x.Otp == otp &&
                !x.IsVerified &&
                x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task UpdateAsync(UserOtp otp)
        {
            _context.UserOtps.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}