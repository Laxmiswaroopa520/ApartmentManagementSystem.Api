
namespace ApartmentManagementSystem.Application.Services
{
    public class OnboardingService
    {
        public async Task CreateInviteAsync(...)
        {
            // Admin only
            // Create UserInvite
            // Generate OTP
            // Save OTP
            // Send OTP
        }

        public async Task VerifyOtpAsync(...)
        {
            // Validate OTP
            // Mark OTP verified
        }

        public async Task CompleteRegistrationAsync(...)
        {
            // Create User
            // Hash password
            // IsActive = true
            // IsRegistrationCompleted = true
        }
    }
}