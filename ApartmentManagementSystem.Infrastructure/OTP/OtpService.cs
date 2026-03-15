using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ApartmentManagementSystem.Infrastructure.OTP
{
    public class OtpService : IOtpService
    {
        private readonly IUnitOfWork UoW;
        private readonly ILogger<OtpService> Logger;

        public OtpService(IUnitOfWork unitOfWork, ILogger<OtpService> logger)
        {
            UoW = unitOfWork;
            Logger = logger;
        }

        /// <summary>
        /// Generates a cryptographically random 6-digit OTP.
        /// </summary>
        public string GenerateOtp()
            => RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        /// <summary>
        /// Validates OTP by phone number. Marks as used and saves via UoW if valid.
        /// </summary>
        public async Task<bool> ValidateOtpAsync(string phoneNumber, string otpCode)
        {
            var otp = await UoW.UserOtps.GetValidOtpAsync(phoneNumber, otpCode);
            if (otp == null) return false;

            // Mutate in memory
            await UoW.UserOtps.MarkAsUsedAsync(otp.Id);

            // ONE SaveChanges
            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Sends OTP via SMS.
        /// DEV: logs to console. PROD: replace with Twilio / AWS SNS.
        /// </summary>
        public async Task SendOtpAsync(string phone, string otp)
        {
            Logger.LogInformation("[DEV MODE] OTP for {Phone}: {Otp}", phone, otp);
            // TODO: Integrate SMS gateway in production
            await Task.CompletedTask;
        }
    }
}





/*using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ApartmentManagementSystem.Infrastructure.OTP;
public class OtpService : IOtpService
{
    private readonly IUserOtpRepository OtpRepository;
    private readonly ILogger<OtpService> Otplogger;
    private readonly IUserRepository UserRepository;

    public OtpService(IUserOtpRepository otpRepository, ILogger<OtpService> logger, IUserRepository userRepository)
    {
        OtpRepository = otpRepository;
        Otplogger = logger;
        UserRepository = userRepository;

    }
    public string GenerateOtp()
    {
        return RandomNumberGenerator
            .GetInt32(100000, 999999)
            .ToString();
    }
    
    public async Task<bool> ValidateOtpAsync(string phoneNumber, string otpCode)
    {
        var otp = await OtpRepository.GetValidOtpAsync(phoneNumber, otpCode);

        if (otp == null)
            return false;

        await OtpRepository.MarkAsUsedAsync(otp.Id);
        return true;
    }
    public async Task<bool> ValidateOtpAsync(Guid userId, string otpCode)
    {
        //  Get user first to get phone number
        var user = await UserRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        //Use YOUR method GetValidOtpAsync which uses PhoneNumber
        var otp = await OtpRepository.GetValidOtpAsync(user.PrimaryPhone, otpCode);

        if (otp == null)
            return false;

        // Mark as used
        await OtpRepository.MarkAsUsedAsync(otp.Id);

        return true;
    }

    public async Task SendOtpAsync(string phone, string otp)
    {
        // DEV MODE: Just log it
        // PRODUCTION: Integrate with SMS gateway (Twilio, AWS SNS, etc.)

        Otplogger.LogInformation($"[DEV MODE] OTP for {phone}: {otp}");

        // TODO: Implement actual SMS sending in production
        await Task.CompletedTask;
    }
}
*/