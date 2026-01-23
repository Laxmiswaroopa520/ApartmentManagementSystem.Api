using ApartmentManagementSystem.Application.Interfaces.Repositories;
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
