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
            RoleId = role.Id,
            ResidentType = (ResidentType)request.ResidentType,
            Status = ResidentStatus.PendingOtpVerification,
            IsActive = true,
            IsOtpVerified = false,
            IsRegistrationCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

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
            await InviteRepo.UpdateStatusAsync(invite.Id, InviteStatus.OtpVerified); // ✅ FIXED
        }

        return new VerifyOtpResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Message = SuccessMessages.OtpVerified,
            Success = true // ✅ ADDED
        };
    }
    /*
    public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto request)
    {
        var user = await _userRepository.GetByPhoneAsync(request.PrimaryPhone);
        if (user == null)
            throw new Exception(ErrorMessages.UserNotFound);

        var isValid = await _otpService.ValidateOtpAsync(user.Id, request.OtpCode);
        if (!isValid)
            throw new Exception(ErrorMessages.InvalidOtp);

        // Update status
        user.IsOtpVerified = true;
        user.Status = ResidentStatus.PendingRegistrationCompletion;

        // Use YOUR pattern: UpdateAsync (which internally saves)
        await _userRepository.UpdateAsync(user);

        // Update invite
        var invite = await _inviteRepository.GetByPhoneAsync(request.PrimaryPhone);
        if (invite != null)
        {
            await _inviteRepository.UpdateStatusAsync(invite.Id, InviteStatus.OtpVerified);
        }

        return new VerifyOtpResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Message = SuccessMessages.OtpVerified,
            Success = true
        };
    }
    */
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

    private async Task<Role?> GetRoleForResidentType(ResidentType residentType)
    {
        var roleName = residentType switch
        {
             ResidentType.Owner => UserRole.ResidentOwner,
          //  ResidentType.Owner => UserRole.Owner,

            ResidentType.Tenant => UserRole.Tenant,
            ResidentType.Staff => UserRole.Security,
            _ => null
        };

        if (roleName == null) return null;
        return await RoleRepository.GetByNameAsync(roleName);
    }
}
































/*
public class OnboardingService : IOnboardingService
{
    private readonly IUserRepository _users;
    private readonly IUserInviteRepository _invites;
    private readonly IUserOtpRepository _otps;
    private readonly IRoleRepository _roles;
    private readonly IOtpService _otpService;
    private readonly IFlatRepository _flatRepository;
    private readonly IUserFlatMappingRepository _userFlatMappingRepository;

    private readonly IFloorRepository _floorRepository;


    public OnboardingService(
    IUserRepository users,
    IUserInviteRepository invites,
    IUserOtpRepository otps,
    IRoleRepository roles,
    IOtpService otpService,
    IFlatRepository flatRepository,
    IUserFlatMappingRepository userFlatMappingRepository,
    IFloorRepository floorRepository)
    {
        _users = users;
        _invites = invites;
        _otps = otps;
        _roles = roles;
        _otpService = otpService;
        _flatRepository = flatRepository;
        _userFlatMappingRepository = userFlatMappingRepository;
        _floorRepository = floorRepository;
    }


    public async Task<CreateInviteResponseDto> CreateInviteAsync(
    CreateUserInviteDto request,
    Guid loggedInUserId)
    {
        var role = await _roles.GetByIdAsync(request.RoleId)
            ?? throw new Exception("Invalid role");

        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            PrimaryPhone = request.PrimaryPhone,
            RoleId = role.Id,
            InviteStatus = "Pending",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = loggedInUserId,
            UserId = Guid.Empty,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _invites.AddAsync(invite);
        await _invites.SaveChangesAsync();

        //  GENERATE OTP
        var otpCode = _otpService.GenerateOtp();

        var otp = new UserOtp
        {
            Id = Guid.NewGuid(),
          //  UserId = invite.Id, // or temp user id if applicable
         PhoneNumber=request.PrimaryPhone,
            OtpCode = otpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        };

        await _otps.AddAsync(otp);
        await _invites.SaveChangesAsync();

        return new CreateInviteResponseDto
        {
            InviteId = invite.Id,
            FullName = invite.FullName,
            PrimaryPhone = invite.PrimaryPhone,
            OtpCode = otpCode,
            OtpExpiresAt = otp.ExpiresAt
        };
    }


    public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto)
    {
        var otp = await _otps.GetValidOtpAsync(dto.PrimaryPhone, dto.OtpCode)
    ?? throw new Exception("Invalid or expired OTP");

        // Mark OTP used
        await _otps.MarkAsUsedAsync(otp.Id);

        // Mark invite as OTP verified (optional but recommended)
        var invite = await _invites.GetByPhoneAsync(dto.PrimaryPhone);
        invite.IsOtpVerified = true;
        await _invites.SaveChangesAsync();

        /*  return new VerifyOtpResponseDto
          {
              PhoneNumber = dto.PrimaryPhone,
              IsVerified = true
          };-----
return new VerifyOtpResponseDto
        {
            PrimaryPhone = dto.PrimaryPhone,
            IsVerified = true
        };
    }
    // =========================
    // FLOORS & FLATS (ONBOARDING)
    // =========================

    public async Task<List<FloorDto>> GetFloorsAsync()
    {
        var floors = await _floorRepository.GetAllAsync();

        return floors.Select(f => new FloorDto
        {
            Id = f.Id,
            FloorNumber = f.FloorNumber
        }).ToList();
    }

    public async Task<List<FlatDto>> GetAvailableFlatsByFloorAsync(Guid floorId)
    {
        var flats = await _flatRepository.GetAvailableFlatsByFloorAsync(floorId);

        return flats.Select(f => new FlatDto
        {
            Id = f.Id,
            FlatNumber = f.FlatNumber
        }).ToList();
    }


    // =========================
    // COMPLETE REGISTRATION
    // =========================
    public async Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(
    CompleteRegistrationDto dto)
    {
        // 1️⃣ Validate invite
        var invite = await _invites.GetByPhoneAsync(dto.PrimaryPhone)
            ?? throw new Exception("Invite not found");

        if (!invite.IsOtpVerified)
            throw new Exception("OTP not verified");

        // 2️⃣ Prevent duplicate user
        var existingUser = await _users.GetByPhoneAsync(dto.PrimaryPhone);
        if (existingUser != null)
            throw new Exception("User already exists");

        // 3️⃣ Create User
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = invite.FullName,
            PrimaryPhone = dto.PrimaryPhone,
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = invite.RoleId,
            IsActive = true,
            IsRegistrationCompleted = true,
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddAsync(user);

        // 4️⃣ LOAD ROLE (IMPORTANT)
        /*  var role = await _roles.GetByIdAsync(invite.RoleId)
              ?? throw new Exception("Role not found");----
        var role = await _roles.GetByIdAsync(invite.RoleId);
        if (role == null)
            throw new Exception("Role not found");


        // =========================
        // 5️⃣ FLAT ASSIGNMENT LOGIC
        // =========================

       //Resident owner
        if (role.Name == "ResidentOwner")
        {
            if (!dto.FlatId.HasValue)
                throw new Exception("Flat selection is required");

            var flat = await _flatRepository.GetByIdAsync(dto.FlatId.Value)
                ?? throw new Exception("Flat not found");

            if (flat.OwnerUserId != null)
                throw new Exception("Flat already assigned");

            flat.OwnerUserId = user.Id;
            flat.UpdatedAt = DateTime.UtcNow;

            await _flatRepository.UpdateAsync(flat);
            await _flatRepository.SaveChangesAsync();
        }


        // 🔹 TENANT
        if (role.Name == "Tenant")
        {
            if (!dto.FlatId.HasValue)
                throw new Exception("Flat selection is required");

            var mapping = new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FlatId = dto.FlatId.Value,
                RelationshipType = "Tenant",
                FromDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userFlatMappingRepository.AddAsync(mapping);
        }

        // 6️⃣ Update Invite
        invite.UserId = user.Id;
        invite.InviteStatus = "Completed";

        // 7️⃣ SAVE ALL
        await _users.SaveChangesAsync();
        await _invites.SaveChangesAsync();

        return new CompleteRegistrationResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Message = "Registration completed successfully"
        };
    }

*/












/*
public async Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(
    CompleteRegistrationDto dto)
{
    // Validate invite exists
    var invite = await _invites.GetByPhoneAsync(dto.PrimaryPhone)
        ?? throw new Exception("Invite not found");

    // Ensure OTP was verified
    if (!invite.IsOtpVerified)
        throw new Exception("OTP not verified");

    // Prevent duplicate registration
    var existingUser = await _users.GetByPhoneAsync(dto.PrimaryPhone);
    if (existingUser != null)
        throw new Exception("User already exists");

    //  Create new user

    var user = new User
    {
        Id = Guid.NewGuid(),
        FullName = invite.FullName,
        PrimaryPhone = dto.PrimaryPhone,   
        Email = dto.Email,
        Username = dto.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        RoleId = invite.RoleId,
        IsActive = true,
        IsRegistrationCompleted = true,
        CreatedAt = DateTime.UtcNow
    };

    await _users.AddAsync(user);

    //  Update invite status
    invite.UserId = user.Id;
    invite.InviteStatus = "Completed";

    await _users.SaveChangesAsync();
    await _invites.SaveChangesAsync();

    return new CompleteRegistrationResponseDto
    {
        UserId = user.Id,
        Username = user.Username
    };
}
*/
















