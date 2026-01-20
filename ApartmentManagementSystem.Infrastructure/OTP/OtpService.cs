// ApartmentManagementSystem.Infrastructure/OTP/OtpService.cs
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ApartmentManagementSystem.Infrastructure.OTP;
/*
public class OtpService : IOtpService
{
    private readonly IUserOtpRepository _otpRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<OtpService> _logger;

    public OtpService(IUserOtpRepository otpRepository, IUserRepository userRepository)
    {
        _otpRepository = otpRepository;
        _userRepository = userRepository;
    }

    public string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public async Task<bool> ValidateOtpAsync(Guid userId, string otpCode)
    {
        // ✅ FIXED: Get user first to get phone number
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        // ✅ FIXED: Use YOUR method GetValidOtpAsync which uses PhoneNumber
        var otp = await _otpRepository.GetValidOtpAsync(user.PrimaryPhone, otpCode);

        if (otp == null)
            return false;

        // Mark as used
        await _otpRepository.MarkAsUsedAsync(otp.Id);

        return true;
    }
    public async Task SendOtpAsync(string phone, string otp)
    {
        // DEV MODE: Just log it
        // PRODUCTION: Integrate with SMS gateway (Twilio, AWS SNS, etc.)

        _logger.LogInformation($"[DEV MODE] OTP for {phone}: {otp}");

        // TODO: Implement actual SMS sending in production
        await Task.CompletedTask;
    }
}
*/





public class OtpService : IOtpService
{
    private readonly IUserOtpRepository _otpRepository;
    private readonly ILogger<OtpService> _logger;
    private readonly IUserRepository _userRepository;

    public OtpService(IUserOtpRepository otpRepository, ILogger<OtpService> logger, IUserRepository userRepository)
    {
        _otpRepository = otpRepository;
        _logger = logger;
        _userRepository = userRepository;

    }
    public string GenerateOtp()
    {
        return RandomNumberGenerator
            .GetInt32(100000, 999999)
            .ToString();
    }
    /*  public string GenerateOtp()
      {
          // Generate 6-digit OTP
          var random = new Random();
          return random.Next(100000, 999999).ToString();
      }
    */
    /*  public async Task<bool> ValidateOtpAsync(Guid userId, string otpCode)
      {
          var otp = await _otpRepository.GetValidOtpAsync(userId, otpCode);

          if (otp == null)
              return false;

          // Mark OTP as used
          await _otpRepository.MarkAsUsedAsync(otp.Id);

          return true;
      }*/
    public async Task<bool> ValidateOtpAsync(string phoneNumber, string otpCode)
    {
        var otp = await _otpRepository.GetValidOtpAsync(phoneNumber, otpCode);

        if (otp == null)
            return false;

        await _otpRepository.MarkAsUsedAsync(otp.Id);
        return true;
    }
    public async Task<bool> ValidateOtpAsync(Guid userId, string otpCode)
    {
        // ✅ FIXED: Get user first to get phone number
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        // ✅ FIXED: Use YOUR method GetValidOtpAsync which uses PhoneNumber
        var otp = await _otpRepository.GetValidOtpAsync(user.PrimaryPhone, otpCode);

        if (otp == null)
            return false;

        // Mark as used
        await _otpRepository.MarkAsUsedAsync(otp.Id);

        return true;
    }

    public async Task SendOtpAsync(string phone, string otp)
    {
        // DEV MODE: Just log it
        // PRODUCTION: Integrate with SMS gateway (Twilio, AWS SNS, etc.)

        _logger.LogInformation($"[DEV MODE] OTP for {phone}: {otp}");

        // TODO: Implement actual SMS sending in production
        await Task.CompletedTask;
    }
}
