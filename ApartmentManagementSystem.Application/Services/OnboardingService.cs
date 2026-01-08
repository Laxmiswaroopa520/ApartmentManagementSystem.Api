
namespace ApartmentManagementSystem.Application.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserInviteRepository _inviteRepo;
        private readonly IUserOtpRepository _otpRepo;
        private readonly IOtpService _otpService;

        public OnboardingService(
            IUserRepository userRepo,
            IUserInviteRepository inviteRepo,
            IUserOtpRepository otpRepo,
            IOtpService otpService)
        {
            _userRepo = userRepo;
            _inviteRepo = inviteRepo;
            _otpRepo = otpRepo;
            _otpService = otpService;
        }

        public async Task CreateInviteAsync(CreateInviteRequest request)
        {
            var existingUser = await _userRepo.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("User already exists");

            var invite = new UserInvite
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Mobile = request.Mobile,
                RoleId = request.RoleId,
                ExpiresAt = DateTime.UtcNow.AddDays(2),
                IsUsed = false
            };

            await _inviteRepo.AddAsync(invite);

            var otpCode = _otpService.GenerateOtp();

            var otp = new UserOtp
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Otp = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsVerified = false
            };

            await _otpRepo.AddAsync(otp);

            await _otpService.SendOtpAsync(request.Mobile, otpCode);
        }

        public async Task VerifyOtpAsync(VerifyOtpRequest request)
        {
            var otp = await _otpRepo.GetValidOtpAsync(request.Email, request.Otp);
            if (otp == null)
                throw new Exception("Invalid or expired OTP");

            otp.IsVerified = true;
            await _otpRepo.UpdateAsync(otp);
        }

        public async Task CompleteRegistrationAsync(CompleteRegistrationRequest request)
        {
            var invite = await _inviteRepo.GetValidInviteAsync(request.Email);
            if (invite == null)
                throw new Exception("Invite not found or expired");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = invite.Email,
                Mobile = invite.Mobile,
                PasswordHash = passwordHash,
                RoleId = invite.RoleId,
                IsActive = true,
                IsRegistrationCompleted = true
            };

            await _userRepo.AddAsync(user);

            invite.IsUsed = true;
            await _inviteRepo.UpdateAsync(invite);
        }
    }
}