
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Infrastructure.Persistence;
   // using global::ApartmentManagementSystem.Application.Interfaces.Repositories;
   // using global::ApartmentManagementSystem.Domain.Entities;
   // using global::ApartmentManagementSystem.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

   // namespace ApartmentManagementSystem.Infrastructure.Repositories;

    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly AppDbContext _db;
        public UserOtpRepository(AppDbContext db) => _db = db;

        public async Task<UserOtp?> GetValidOtpAsync(string phone, string otp)
        {
            return await _db.UserOtps.FirstOrDefaultAsync(x =>
                x.PhoneNumber == phone&&
                x.OtpCode == otp &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);
        }

        public async Task AddAsync(UserOtp otp)
            => await _db.UserOtps.AddAsync(otp);

        public async Task MarkAsUsedAsync(Guid otpId)
        {
            var otp = await _db.UserOtps.FindAsync(otpId);
            otp!.IsUsed = true;
            await _db.SaveChangesAsync();
        }
    }
}