using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Domain.Entities;
using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IUserRepository UserRepo;
        private readonly IUserInviteRepository InviteRepo;
        private readonly IUserOtpRepository OtpRepo;
        private readonly IOtpService OtpService;

        public OnboardingService(
            IUserRepository userRepo,
            IUserInviteRepository inviteRepo,
            IUserOtpRepository otpRepo,
            IOtpService otpService)
        {
            UserRepo = userRepo;
            InviteRepo = inviteRepo;
            OtpRepo = otpRepo;
            OtpService = otpService;
        }

        public async Task CreateInviteAsync(CreateInviteRequest request)
        {
            var existingUser = await UserRepo.GetByEmailAsync(request.Email);
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

            await InviteRepo.AddAsync(invite);

            var otpCode = OtpService.GenerateOtp();

            var otp = new UserOtp
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Otp = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsVerified = false
            };

            await OtpRepo.AddAsync(otp);

            await OtpService.SendOtpAsync(request.Mobile, otpCode);
        }

        public async Task VerifyOtpAsync(VerifyOtpRequest request)
        {
            var otp = await OtpRepo.GetValidOtpAsync(request.Email, request.Otp);
            if (otp == null)
                throw new Exception("Invalid or expired OTP");

            otp.IsVerified = true;
            await OtpRepo.UpdateAsync(otp);
        }

        public async Task CompleteRegistrationAsync(CompleteRegistrationRequest request)
        {
            var invite = await InviteRepo.GetValidInviteAsync(request.Email);
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

            await UserRepo.AddAsync(user);

            invite.IsUsed = true;
            await InviteRepo.UpdateAsync(invite);
        }
    }
}