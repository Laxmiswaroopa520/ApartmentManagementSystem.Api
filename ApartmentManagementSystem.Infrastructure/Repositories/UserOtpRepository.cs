
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly AppDbContext DBContext;
        public UserOtpRepository(AppDbContext db) => DBContext = db;

        public async Task<UserOtp?> GetValidOtpAsync(string phone, string otp)
        {
            return await DBContext.UserOtps.FirstOrDefaultAsync(x =>
                x.PhoneNumber == phone&&
                x.OtpCode == otp &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task AddAsync(UserOtp otp)
            => await DBContext.UserOtps.AddAsync(otp);

        public async Task MarkAsUsedAsync(Guid otpId)
        {
            var otp = await DBContext.UserOtps.FindAsync(otpId);
            otp!.IsUsed = true;
            await DBContext.SaveChangesAsync();
        }
      /*  public async Task<UserOtp?> GetLatestByUserIdAsync(Guid userId)
        {
            return await _db.UserOtps
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }*/
    }
}