using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for the resident onboarding flow.
    ///
    /// Handles the three-step registration process:
    /// 1. CreateInvite — validates phone, creates user + OTP + invite in one SaveChanges.
    /// 2. VerifyOtp   — validates OTP, updates user status, updates invite status.
    /// 3. CompleteRegistration — sets credentials, moves user to PendingFlatAllocation.
    /// </summary>
    public class OnboardingService : IOnboardingService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>OTP service for generating and validating one-time passwords.</summary>
        private readonly IOtpService OtpService;

        /// <summary>Email service for sending admin notifications.</summary>
        private readonly IEmailService EmailService;

        /// <summary>
        /// Initialises OnboardingService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        /// <param name="otpService">Service for OTP generation and validation.</param>
        /// <param name="emailService">Service for email notifications.</param>
        public OnboardingService(
            IUnitOfWork unitOfWork,
            IOtpService otpService,
            IEmailService emailService)
        {
            UoW = unitOfWork;
            OtpService = otpService;
            EmailService = emailService;
        }

        /// <summary>
        /// Step 1 — Creates an invitation for a new resident.
        ///
        /// Actions performed in one SaveChanges:
        /// - Creates a new User record with PendingOtpVerification status.
        /// - Assigns the appropriate role based on resident type.
        /// - Creates a UserOtp record with a 10-minute expiry.
        /// - Creates a UserInvite tracking record.
        /// </summary>
        /// <param name="request">Invite details including phone number and resident type.</param>
        /// <param name="createdByUserId">UserId of the admin creating the invite.</param>
        /// <returns>Response DTO containing the generated OTP code and user details.</returns>
        /// <exception cref="Exception">
        /// Thrown when the phone number already exists or the resident type is invalid.
        /// </exception>
        public async Task<CreateInviteResponseDto> CreateInviteAsync(
            CreateUserInviteDto request, Guid createdByUserId)
        {
            if (await UoW.Users.PhoneExistsAsync(request.PrimaryPhone))
                throw new Exception(ErrorMessages.PhoneAlreadyExists);

            var role = await GetRoleForResidentType((ResidentType)request.ResidentType);
            if (role == null)
                throw new Exception(ResidentMessages.InvalidResident);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                PrimaryPhone = request.PrimaryPhone,
                ResidentType = (ResidentType)request.ResidentType,
                Status = ResidentStatus.PendingOtpVerification,
                IsActive = true,
                IsOtpVerified = false,
                IsRegistrationCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            await UoW.Users.AddAsync(user);

            var otpCode = OtpService.GenerateOtp();
            await UoW.UserOtps.AddAsync(new UserOtp
            {
                Id = Guid.NewGuid(),
                PhoneNumber = user.PrimaryPhone,
                OtpCode = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            });

            await UoW.UserInvites.AddAsync(new UserInvite
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                PrimaryPhone = request.PrimaryPhone,
                RoleId = role.Id,
                ResidentType = (ResidentType)request.ResidentType,
                InviteStatus = InviteStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = createdByUserId
            });

            // ONE SaveChanges for user + OTP + invite
            await UoW.SaveChangesAsync();

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

        /// <summary>
        /// Step 2 — Verifies the OTP entered by the resident.
        ///
        /// On success:
        /// - Marks the user as OTP-verified.
        /// - Advances user status to PendingRegistrationCompletion.
        /// - Updates the invite record to OtpVerified.
        ///
        /// All changes committed in one SaveChanges.
        /// </summary>
        /// <param name="request">OTP verification request with phone number and OTP code.</param>
        /// <returns>Verification response with user details and success flag.</returns>
        /// <exception cref="Exception">Thrown when user is not found or OTP is invalid/expired.</exception>
        public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto request)
        {
            var user = await UoW.Users.GetByPhoneAsync(request.PrimaryPhone)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var isValid = await OtpService.ValidateOtpAsync(user.PrimaryPhone, request.OtpCode);
            if (!isValid)
                throw new Exception(ErrorMessages.InvalidOtp);

            user.IsOtpVerified = true;
            user.Status = ResidentStatus.PendingRegistrationCompletion;
            UoW.Users.Update(user);

            var invite = await UoW.UserInvites.GetByPhoneAsync(request.PrimaryPhone);
            if (invite != null)
                invite.InviteStatus = InviteStatus.OtpVerified;

            // ONE SaveChanges for user + invite
            await UoW.SaveChangesAsync();

            return new VerifyOtpResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Message = SuccessMessages.OtpVerified,
                Success = true
            };
        }

        /// <summary>
        /// Step 3 — Completes registration by setting login credentials and profile data.
        ///
        /// On success:
        /// - Updates user with username, hashed password, email, and secondary phone.
        /// - Sets IsRegistrationCompleted to true.
        /// - Advances user status to PendingFlatAllocation.
        /// - Marks invite as Completed.
        /// - Sends a registration completion notification email to the admin.
        ///
        /// All changes committed in one SaveChanges.
        /// </summary>
        /// <param name="request">Registration completion data including username and password.</param>
        /// <returns>Response DTO with userId, username, and status.</returns>
        /// <exception cref="Exception">
        /// Thrown when user not found, OTP not verified, or username already taken.
        /// </exception>
        public async Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(
            CompleteRegistrationDto request)
        {
            var user = await UoW.Users.GetByPhoneAsync(request.PrimaryPhone)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            if (!user.IsOtpVerified)
                throw new Exception(OtpMessages.OtpNotVerified);

            if (await UoW.Users.UsernameExistsAsync(request.Username))
                throw new Exception(ErrorMessages.UsernameAlreadyExists);

            user.FullName = request.FullName;
            user.SecondaryPhone = request.SecondaryPhone;
            user.Email = request.Email;
            user.Username = request.Username;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.IsRegistrationCompleted = true;
            user.Status = ResidentStatus.PendingFlatAllocation;
            user.FlatId = null;
            UoW.Users.Update(user);

            var invite = await UoW.UserInvites.GetByPhoneAsync(request.PrimaryPhone);
            if (invite != null)
                invite.InviteStatus = InviteStatus.Completed;

            // ONE SaveChanges for user + invite
            await UoW.SaveChangesAsync();

            await EmailService.SendRegistrationCompletedToAdminAsync(
                user.FullName,
                user.PrimaryPhone,
                user.ResidentType?.ToString() ?? "Unknown");

            return new CompleteRegistrationResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Status = "PendingFlatAllocation",
                Message = SuccessMessages.RegistrationCompleted
            };
        }


        /// <summary>
        /// Resolves the correct system role name for a given resident type
        /// and retrieves the corresponding Role entity from the database.
        /// </summary>
        /// <param name="residentType">Resident type enum value.</param>
        /// <returns>Role entity or null when the type has no matching role.</returns>
        private async Task<Role?> GetRoleForResidentType(ResidentType residentType)
        {
            var roleName = residentType switch
            {
                ResidentType.Owner => RoleNames.ResidentOwner,
                ResidentType.Tenant => RoleNames.Tenant,
                ResidentType.Staff => RoleNames.Staff,
                _ => null
            };

            if (roleName == null) return null;
            return await UoW.Roles.GetByNameAsync(roleName);
        }
    }
}
























/*using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly IUnitOfWork UoW;
        private readonly IOtpService OtpService;
        private readonly IEmailService EmailService;

        public OnboardingService(
            IUnitOfWork unitOfWork,
            IOtpService otpService,
            IEmailService emailService)
        {
            UoW = unitOfWork;
            OtpService = otpService;
            EmailService = emailService;
        }

        public async Task<CreateInviteResponseDto> CreateInviteAsync(
            CreateUserInviteDto request, Guid createdByUserId)
        {
            if (await UoW.Users.PhoneExistsAsync(request.PrimaryPhone))
                throw new Exception(ErrorMessages.PhoneAlreadyExists);

            var role = await GetRoleForResidentType((ResidentType)request.ResidentType);
            if (role == null)
                throw new Exception(ResidentMessages.InvalidResident);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                PrimaryPhone = request.PrimaryPhone,
                ResidentType = (ResidentType)request.ResidentType,
                Status = ResidentStatus.PendingOtpVerification,
                IsActive = true,
                IsOtpVerified = false,
                IsRegistrationCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });

            // Stage user
            await UoW.Users.AddAsync(user);

            // Stage OTP
            var otpCode = OtpService.GenerateOtp();
            await UoW.UserOtps.AddAsync(new UserOtp
            {
                Id = Guid.NewGuid(),
                PhoneNumber = user.PrimaryPhone,
                OtpCode = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            });

            // Stage invite
            await UoW.UserInvites.AddAsync(new UserInvite
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                PrimaryPhone = request.PrimaryPhone,
                RoleId = role.Id,
                ResidentType = (ResidentType)request.ResidentType,
                InviteStatus = InviteStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = createdByUserId
            });

            // ONE SaveChanges for user + otp + invite
            await UoW.SaveChangesAsync();

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
            var user = await UoW.Users.GetByPhoneAsync(request.PrimaryPhone)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var isValid = await OtpService.ValidateOtpAsync(user.PrimaryPhone, request.OtpCode);
            if (!isValid)
                throw new Exception(ErrorMessages.InvalidOtp);

            // Mutate in memory
            user.IsOtpVerified = true;
            user.Status = ResidentStatus.PendingRegistrationCompletion;
            UoW.Users.Update(user);

            var invite = await UoW.UserInvites.GetByPhoneAsync(request.PrimaryPhone);
            if (invite != null)
            {
                invite.InviteStatus = InviteStatus.OtpVerified;
                // Update is tracked by EF — no explicit call needed
            }

            // ONE SaveChanges for user + invite
            await UoW.SaveChangesAsync();

            return new VerifyOtpResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Message = SuccessMessages.OtpVerified,
                Success = true
            };
        }

        public async Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(
            CompleteRegistrationDto request)
        {
            var user = await UoW.Users.GetByPhoneAsync(request.PrimaryPhone)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            if (!user.IsOtpVerified)
                throw new Exception(OtpMessages.OtpNotVerified);

            if (await UoW.Users.UsernameExistsAsync(request.Username))
                throw new Exception(ErrorMessages.UsernameAlreadyExists);

            // Mutate in memory
            user.FullName = request.FullName;
            user.SecondaryPhone = request.SecondaryPhone;
            user.Email = request.Email;
            user.Username = request.Username;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.IsRegistrationCompleted = true;
            user.Status = ResidentStatus.PendingFlatAllocation;
            user.FlatId = null;
            UoW.Users.Update(user);

            var invite = await UoW.UserInvites.GetByPhoneAsync(request.PrimaryPhone);
            if (invite != null)
                invite.InviteStatus = InviteStatus.Completed;

            // ONE SaveChanges for user + invite
            await UoW.SaveChangesAsync();

            await EmailService.SendRegistrationCompletedToAdminAsync(
                user.FullName,
                user.PrimaryPhone,
                user.ResidentType?.ToString() ?? "Unknown");

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
                ResidentType.Owner => RoleNames.ResidentOwner,
                ResidentType.Tenant => RoleNames.Tenant,
                ResidentType.Staff => RoleNames.Staff,
                _ => null
            };

            if (roleName == null) return null;
            return await UoW.Roles.GetByNameAsync(roleName);
        }
    }
}




*/







/*using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
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
            throw new Exception(ResidentMessages.InvalidResident);

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
            throw new Exception(OtpMessages.OtpNotVerified);

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
    //staff  check here..
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

*/













































