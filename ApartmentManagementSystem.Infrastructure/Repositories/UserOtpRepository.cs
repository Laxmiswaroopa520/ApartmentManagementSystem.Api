namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    using ApartmentManagementSystem.Application.Interfaces.Repositories;
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository responsible for handling data access operations
    /// related to User OTP (One-Time Password) records.
    /// 
    /// Handles:
    /// - Retrieving valid OTPs
    /// - Adding new OTP records
    /// - Marking OTPs as used
    /// </summary>
    public class UserOtpRepository : IUserOtpRepository
    {
        /// <summary>
        /// Database context used for persistence operations.
        /// </summary>
        private readonly AppDbContext DBContext;

        /// <summary>
        /// Constructor for injecting the application database context.
        /// </summary>
        /// <param name="db">
        /// Application database context.
        /// </param>
        public UserOtpRepository(AppDbContext db)
        {
            DBContext = db;
        }

        /// <summary>
        /// Retrieves a valid OTP for a given phone number and OTP code.
        /// 
        /// Validation Conditions:
        /// - Phone number must match
        /// - OTP code must match
        /// - OTP must not be used
        /// - OTP must not be expired
        /// </summary>
        /// <param name="phone">
        /// Phone number associated with the OTP.
        /// </param>
        /// <param name="otp">
        /// OTP code entered by the user.
        /// </param>
        /// <returns>
        /// Valid UserOtp entity if found; otherwise null.
        /// </returns>
        public async Task<UserOtp?> GetValidOtpAsync(string phone, string otp)
        {
            return await DBContext.UserOtps.FirstOrDefaultAsync(x =>
                x.PhoneNumber == phone &&
                x.OtpCode == otp &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);
        }

        /// <summary>
        /// Adds a new OTP record to the database.
        /// 
        /// Note: Changes are not saved immediately.
        /// Caller must invoke SaveChangesAsync() if required.
        /// </summary>
        /// <param name="otp">
        /// UserOtp entity to be added.
        /// </param>
        public async Task AddAsync(UserOtp otp)
        {
            await DBContext.UserOtps.AddAsync(otp);
        }

        /// <summary>
        /// Marks an OTP as used to prevent reuse.
        /// </summary>
        /// <param name="otpId">
        /// Unique identifier of the OTP record.
        /// </param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task MarkAsUsedAsync(Guid otpId)
        {
            var otp = await DBContext.UserOtps.FindAsync(otpId);

            if (otp != null)
            {
                otp.IsUsed = true;
                await DBContext.SaveChangesAsync();
            }
        }
    }
}















/*
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
    }
}
*/