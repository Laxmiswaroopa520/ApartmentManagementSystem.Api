using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Entities;
using BCrypt.Net;

namespace ApartmentManagementSystem.Application.Services;

public class OnboardingService : IOnboardingService
{
    private readonly IUserRepository _users;
    private readonly IUserInviteRepository _invites;
    private readonly IUserOtpRepository _otps;
    private readonly IRoleRepository _roles;
    private readonly IOtpService _otpService;

    public OnboardingService(
        IUserRepository users,
        IUserInviteRepository invites,
        IUserOtpRepository otps,
        IRoleRepository roles,
        IOtpService otpService)
    {
        _users = users;
        _invites = invites;
        _otps = otps;
        _roles = roles;
        _otpService = otpService;
    }

    public async Task<CreateInviteResponseDto> CreateInviteAsync(
        CreateUserInviteDto dto,
        Guid createdBy)
    {
        var role = await _roles.GetByIdAsync(dto.RoleId);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            PrimaryPhone = dto.PrimaryPhone,
            RoleId = role.Id,
            IsActive = false
        };

        await _users.AddAsync(user);

        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(2)
        };

        await _invites.AddAsync(invite);

        await _users.SaveChangesAsync();

        return new CreateInviteResponseDto
        {
            UserId = user.Id,
            PrimaryPhone = user.PrimaryPhone,
            ExpiresAt = invite.ExpiresAt
        };
    }

    public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
    {
        var user = await _users.GetByPhoneAsync(dto.PrimaryPhone)
            ?? throw new Exception("User not found");

        var otp = await _otps.GetValidOtpAsync(user.Id, dto.OtpCode)
            ?? throw new Exception("Invalid OTP");

        await _otps.MarkAsUsedAsync(otp.Id);

        return new VerifyOtpResponseDto
        {
            UserId = user.Id,
            IsVerified = true
        };
    }

    public async Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(
        CompleteRegistrationDto dto)
    {
        var user = await _users.GetByPhoneAsync(dto.PrimaryPhone)
            ?? throw new Exception("User not found");

        user.Username = dto.Username;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.IsActive = true;

        await _users.SaveChangesAsync();

        return new CompleteRegistrationResponseDto
        {
            UserId = user.Id,
            Username = user.Username
        };
    }
}
