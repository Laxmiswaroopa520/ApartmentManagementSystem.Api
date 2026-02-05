using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using System.Runtime.Intrinsics.X86;
using static System.Net.WebRequestMethods;


namespace ApartmentManagementSystem.Application.Services;

public class OnboardingService : IOnboardingService
{
    private readonly IUserRepository UserRepo;
    private readonly IUserInviteRepository InviteRepo;
    private readonly IUserOtpRepository OtpRepo;
    private readonly IOtpService OtpService;
    private readonly IEmailService EmailService;
    private readonly IRoleRepository RoleRepository;

    public OnboardingService(
        IUserRepository userRepository,
        IUserInviteRepository inviteRepository,
        IUserOtpRepository otpRepository,
        IOtpService otpService,
        IEmailService emailService,
        IRoleRepository roleRepository)
    {
        UserRepo = userRepository;
        InviteRepo = inviteRepository;
        OtpRepo = otpRepository;
        OtpService = otpService;
        EmailService = emailService;
        RoleRepository = roleRepository;
    }

    public async Task<CreateInviteResponseDto> CreateInviteAsync(CreateUserInviteDto request, Guid createdByUserId)
    {
        if (await UserRepo.PhoneExistsAsync(request.PrimaryPhone))
            throw new Exception(ErrorMessages.PhoneAlreadyExists);

        // Get role based on resident type
        var role = await GetRoleForResidentType((ResidentType)request.ResidentType);
        if (role == null)
            throw new Exception("Invalid resident type");

        // Create user with PendingOtpVerification status
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            PrimaryPhone = request.PrimaryPhone,
            //RoleId = role.Id,
            ResidentType = (ResidentType)request.ResidentType,
            Status = ResidentStatus.PendingOtpVerification,
            IsActive = true,
            IsOtpVerified = false,
            IsRegistrationCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        // because you removed roleid from the user right Assign role via UserRoles (THIS IS THE KEY FIX)
        user.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        // Use YOUR pattern: AddAsync + SaveChangesAsync
        await UserRepo.AddAsync(user);
        await UserRepo.SaveChangesAsync();

        // Generate OTP
       // var otpCode = _otpService.GenerateOtp();
       /* var otp = new UserOtp
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PhoneNumber = user.PrimaryPhone,
            OtpCode = otpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };*/
       // Generate OTP - Use YOUR UserOtp structure
        var otpCode = OtpService.GenerateOtp();
        var otp = new UserOtp
        {
            Id = Guid.NewGuid(),
            PhoneNumber = user.PrimaryPhone, //Use PhoneNumber not UserId
            OtpCode = otpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await OtpRepo.AddAsync(otp);

        // Create invite record
        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            PrimaryPhone = request.PrimaryPhone,
            RoleId = role.Id,
            ResidentType = (ResidentType)request.ResidentType,
            InviteStatus = InviteStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };

        await InviteRepo.CreateAsync(invite);

        return new CreateInviteResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            PrimaryPhone = user.PrimaryPhone,
            ResidentType = ((ResidentType)request.ResidentType).ToString(),
            OtpCode = otpCode,
            Message = SuccessMessages.InviteCreated
        };
    }

    public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto request)
    {
        var user = await UserRepo.GetByPhoneAsync(request.PrimaryPhone);
        if (user == null)
            throw new Exception(ErrorMessages.UserNotFound);

        // var isValid = await _otpService.ValidateOtpAsync(user.Id, request.OtpCode);
        var isValid = await OtpService.ValidateOtpAsync(user.PrimaryPhone, request.OtpCode);

        if (!isValid)
            throw new Exception(ErrorMessages.InvalidOtp);

        user.IsOtpVerified = true;
        user.Status = ResidentStatus.PendingRegistrationCompletion;
        await UserRepo.UpdateAsync(user);

        var invite = await InviteRepo.GetByPhoneAsync(request.PrimaryPhone);
        if (invite != null)
        {
            await InviteRepo.UpdateStatusAsync(invite.Id, InviteStatus.OtpVerified); 
        }

        return new VerifyOtpResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Message = SuccessMessages.OtpVerified,
            Success = true 
        };
    }
  
    public async Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(CompleteRegistrationDto request)
    {
        var user = await UserRepo.GetByPhoneAsync(request.PrimaryPhone);
        if (user == null)
            throw new Exception(ErrorMessages.UserNotFound);

        if (!user.IsOtpVerified)
            throw new Exception("Please verify OTP first");

        if (await UserRepo.UsernameExistsAsync(request.Username))
            throw new Exception(ErrorMessages.UsernameAlreadyExists);

        // Complete registration WITHOUT flat assignment
        user.FullName = request.FullName;
        user.SecondaryPhone = request.SecondaryPhone;
        user.Email = request.Email;
        user.Username = request.Username;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.IsRegistrationCompleted = true;
        user.Status = ResidentStatus.PendingFlatAllocation;
        user.FlatId = null; // No flat yet

        // Use YOUR pattern
        await UserRepo.UpdateAsync(user);

        // Update invite
        var invite = await InviteRepo.GetByPhoneAsync(request.PrimaryPhone);
        if (invite != null)
        {
            await InviteRepo.UpdateStatusAsync(invite.Id, InviteStatus.Completed);
        }

        // Send email to SuperAdmin
        await EmailService.SendRegistrationCompletedToAdminAsync(
            user.FullName,
            user.PrimaryPhone,
            user.ResidentType?.ToString() ?? "Unknown"
        );

        return new CompleteRegistrationResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Status = "PendingFlatAllocation",
            Message = SuccessMessages.RegistrationCompleted
        };
    }
    //staff   check here..
    private async Task<Role?> GetRoleForResidentType(ResidentType residentType)
    {
        var roleName = residentType switch
        {
            ResidentType.Owner => RoleNames.ResidentOwner,
            //  ResidentType.Owner => UserRole.Owner,

            ResidentType.Tenant => RoleNames.Tenant,
            // ResidentType.Staff => RoleNames.Security,
            ResidentType.Staff => RoleNames.Staff,
            _ => null
        };

        if (roleName == null) return null;
        return await RoleRepository.GetByNameAsync(roleName);
    }
}















































