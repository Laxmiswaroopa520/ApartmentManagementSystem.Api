namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IOtpService
    {
        string GenerateOtp();
        Task<bool> ValidateOtpAsync(string phoneNumber, string otpCode);
        Task SendOtpAsync(string phone, string otp);
    }

}