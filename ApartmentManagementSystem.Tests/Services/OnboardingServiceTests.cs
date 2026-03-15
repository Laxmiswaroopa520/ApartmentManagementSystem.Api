using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.Tests.Services;

public class OnboardingServiceTests
{
    // ── Single mock for UnitOfWork ─────────────────────────────────
    private readonly Mock<IUnitOfWork> MockUoW;

    // ── Individual repo mocks exposed via UoW properties ──────────
    private readonly Mock<IUserRepository> MockUserRepo;
    private readonly Mock<IUserInviteRepository> MockInviteRepo;
    private readonly Mock<IUserOtpRepository> MockOtpRepo;
    private readonly Mock<IRoleRepository> MockRoleRepo;

    // ── Other service mocks ────────────────────────────────────────
    private readonly Mock<IOtpService> MockOtpService;
    private readonly Mock<IEmailService> MockEmailService;

    private readonly OnboardingService OnboardingService;

    public OnboardingServiceTests()
    {
        MockUoW = new Mock<IUnitOfWork>();
        MockUserRepo = new Mock<IUserRepository>();
        MockInviteRepo = new Mock<IUserInviteRepository>();
        MockOtpRepo = new Mock<IUserOtpRepository>();
        MockRoleRepo = new Mock<IRoleRepository>();
        MockOtpService = new Mock<IOtpService>();
        MockEmailService = new Mock<IEmailService>();

        // Wire repo mocks to the UoW mock properties
        MockUoW.Setup(u => u.Users).Returns(MockUserRepo.Object);
        MockUoW.Setup(u => u.UserInvites).Returns(MockInviteRepo.Object);
        MockUoW.Setup(u => u.UserOtps).Returns(MockOtpRepo.Object);
        MockUoW.Setup(u => u.Roles).Returns(MockRoleRepo.Object);

        // SaveChangesAsync always succeeds in tests
        MockUoW.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // OnboardingService now takes (IUnitOfWork, IOtpService, IEmailService)
        OnboardingService = new OnboardingService(
            MockUoW.Object,
            MockOtpService.Object,
            MockEmailService.Object
        );
    }

    // ══════════════════════════════════════════════════════════════
    // CreateInviteAsync
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public async Task CreateInviteAsync_WithValidOwnerData_CreatesInviteSuccessfully()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "John Doe",
            PrimaryPhone = "9876543210",
            ResidentType = (int)ResidentType.Owner
        };
        var createdBy = Guid.NewGuid();
        var role = new Role { Id = Guid.NewGuid(), Name = "ResidentOwner" };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone)).ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync("ResidentOwner")).ReturnsAsync(role);
        MockOtpService.Setup(x => x.GenerateOtp()).Returns("123456");

        // Act
        var result = await OnboardingService.CreateInviteAsync(request, createdBy);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.PrimaryPhone, result.PrimaryPhone);
        Assert.Equal("Owner", result.ResidentType);
        Assert.Equal("123456", result.OtpCode);

        // User staged via AddAsync (not SaveChanges on the repo)
        MockUserRepo.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);

        // OTP staged via UoW.UserOtps.AddAsync
        MockOtpRepo.Verify(x => x.AddAsync(It.IsAny<UserOtp>()), Times.Once);

        // Invite staged via UoW.UserInvites.AddAsync
        MockInviteRepo.Verify(x => x.AddAsync(It.IsAny<UserInvite>()), Times.Once);

        // Single SaveChanges on UoW
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateInviteAsync_WithValidTenantData_CreatesInviteSuccessfully()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "Jane Smith",
            PrimaryPhone = "9876543211",
            ResidentType = (int)ResidentType.Tenant
        };
        var role = new Role { Id = Guid.NewGuid(), Name = "Tenant" };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone)).ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync("Tenant")).ReturnsAsync(role);
        MockOtpService.Setup(x => x.GenerateOtp()).Returns("654321");

        // Act
        var result = await OnboardingService.CreateInviteAsync(request, Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tenant", result.ResidentType);
        Assert.Equal("654321", result.OtpCode);
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateInviteAsync_WithValidStaffData_CreatesInviteSuccessfully()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "Security Guard",
            PrimaryPhone = "9876543212",
            ResidentType = (int)ResidentType.Staff
        };
        var role = new Role { Id = Guid.NewGuid(), Name = "Staff" };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone)).ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync("Staff")).ReturnsAsync(role);
        MockOtpService.Setup(x => x.GenerateOtp()).Returns("111222");

        // Act
        var result = await OnboardingService.CreateInviteAsync(request, Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Staff", result.ResidentType);
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateInviteAsync_WithExistingPhone_ThrowsException()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "John Doe",
            PrimaryPhone = "9876543210",
            ResidentType = (int)ResidentType.Owner
        };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CreateInviteAsync(request, Guid.NewGuid())
        );

        Assert.True(
            exception.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            exception.Message.Contains("Phone", StringComparison.OrdinalIgnoreCase),
            $"Unexpected message: {exception.Message}"
        );

        // SaveChanges must NOT be called when validation fails
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateInviteAsync_WithInvalidResidentType_ThrowsException()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "John Doe",
            PrimaryPhone = "9876543210",
            ResidentType = 999  // invalid
        };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone)).ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Role?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CreateInviteAsync(request, Guid.NewGuid())
        );

        Assert.Equal("Invalid resident type", exception.Message);
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ══════════════════════════════════════════════════════════════
    // VerifyOtpAsync
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public async Task VerifyOtpAsync_WithValidOtp_UpdatesUserStatus()
    {
        // Arrange
        var request = new VerifyOtpDto
        {
            PrimaryPhone = "9876543210",
            OtpCode = "123456"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            FullName = "Test User",
            IsOtpVerified = false,
            Status = ResidentStatus.PendingOtpVerification
        };

        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            InviteStatus = InviteStatus.Pending
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone)).ReturnsAsync(user);
        MockOtpService.Setup(x => x.ValidateOtpAsync(request.PrimaryPhone, request.OtpCode))
            .ReturnsAsync(true);
        MockInviteRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone)).ReturnsAsync(invite);

        // Act
        var result = await OnboardingService.VerifyOtpAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.FullName, result.FullName);

        // User mutated and Update() called (sync, no await)
        MockUserRepo.Verify(x => x.Update(It.Is<User>(u =>
            u.IsOtpVerified == true &&
            u.Status == ResidentStatus.PendingRegistrationCompletion
        )), Times.Once);

        // Single SaveChanges on UoW
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task VerifyOtpAsync_WithNonExistentUser_ThrowsException()
    {
        // Arrange
        var request = new VerifyOtpDto
        {
            PrimaryPhone = "9876543210",
            OtpCode = "123456"
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.VerifyOtpAsync(request)
        );

        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task VerifyOtpAsync_WithInvalidOtp_ThrowsException()
    {
        // Arrange
        var request = new VerifyOtpDto
        {
            PrimaryPhone = "9876543210",
            OtpCode = "999999"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            FullName = "Test User"
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone)).ReturnsAsync(user);
        MockOtpService.Setup(x => x.ValidateOtpAsync(request.PrimaryPhone, request.OtpCode))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.VerifyOtpAsync(request)
        );

        Assert.Contains("invalid", exception.Message, StringComparison.OrdinalIgnoreCase);
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    // ══════════════════════════════════════════════════════════════
    // CompleteRegistrationAsync
    // ══════════════════════════════════════════════════════════════

    [Fact]
    public async Task CompleteRegistrationAsync_WithValidData_CompletesRegistration()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            FullName = "John Doe Updated",
            SecondaryPhone = "9876543211",
            Email = "john@example.com",
            Username = "johndoe",
            Password = "Password@123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            FullName = "John Doe",
            IsOtpVerified = true,
            Status = ResidentStatus.PendingRegistrationCompletion,
            ResidentType = ResidentType.Owner
        };

        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            InviteStatus = InviteStatus.OtpVerified
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone)).ReturnsAsync(user);
        MockUserRepo.Setup(x => x.UsernameExistsAsync(request.Username)).ReturnsAsync(false);
        MockInviteRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone)).ReturnsAsync(invite);

        // Act
        var result = await OnboardingService.CompleteRegistrationAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(request.Username, result.Username);
        Assert.Equal("PendingFlatAllocation", result.Status);

        // User mutated and Update() called
        MockUserRepo.Verify(x => x.Update(It.Is<User>(u =>
            u.Username == request.Username &&
            u.Email == request.Email &&
            u.IsRegistrationCompleted == true &&
            u.Status == ResidentStatus.PendingFlatAllocation
        )), Times.Once);

        // Single SaveChanges on UoW
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Once);

        // Email sent to admin
        MockEmailService.Verify(x => x.SendRegistrationCompletedToAdminAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_WithNonExistentUser_ThrowsException()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            Username = "johndoe",
            Password = "Password@123"
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CompleteRegistrationAsync(request)
        );

        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_WithoutOtpVerification_ThrowsException()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            Username = "johndoe",
            Password = "Password@123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            IsOtpVerified = false
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone)).ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CompleteRegistrationAsync(request)
        );

        Assert.Contains("verify OTP", exception.Message, StringComparison.OrdinalIgnoreCase);
        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_WithExistingUsername_ThrowsException()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            FullName = "John Doe",
            Username = "existinguser",
            Password = "Password@123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            IsOtpVerified = true,
            Status = ResidentStatus.PendingRegistrationCompletion
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone)).ReturnsAsync(user);
        MockUserRepo.Setup(x => x.UsernameExistsAsync(request.Username)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CompleteRegistrationAsync(request)
        );

        Assert.True(
            exception.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            exception.Message.Contains("Username", StringComparison.OrdinalIgnoreCase),
            $"Unexpected message: {exception.Message}"
        );

        MockUoW.Verify(x => x.SaveChangesAsync(), Times.Never);
    }
}













/*using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.Tests.Services;

public class OnboardingServiceTests
{
    private readonly Mock<IUserRepository> MockUserRepo;
    private readonly Mock<IUserInviteRepository> MockInviteRepo;
    private readonly Mock<IUserOtpRepository> MockOtpRepo;
    private readonly Mock<IOtpService> MockOtpService;
    private readonly Mock<IEmailService> MockEmailService;
    private readonly Mock<IRoleRepository> MockRoleRepo;
    private readonly OnboardingService OnboardingService;

    public OnboardingServiceTests()
    {
        MockUserRepo = new Mock<IUserRepository>();
        MockInviteRepo = new Mock<IUserInviteRepository>();
        MockOtpRepo = new Mock<IUserOtpRepository>();
        MockOtpService = new Mock<IOtpService>();
        MockEmailService = new Mock<IEmailService>();
        MockRoleRepo = new Mock<IRoleRepository>();

        OnboardingService = new OnboardingService(
            MockUserRepo.Object,
            MockInviteRepo.Object,
            MockOtpRepo.Object,
            MockOtpService.Object,
            MockEmailService.Object,
            MockRoleRepo.Object
        );
    }

    #region CreateInviteAsync Tests

    [Fact]
    public async Task CreateInviteAsync_WithValidOwnerData_CreatesInviteSuccessfully()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "John Doe",
            PrimaryPhone = "9876543210",
            ResidentType = (int)ResidentType.Owner
        };
        var createdByUserId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "ResidentOwner" };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync("ResidentOwner"))
            .ReturnsAsync(role);
        MockOtpService.Setup(x => x.GenerateOtp())
            .Returns("123456");

        // Act
        var result = await OnboardingService.CreateInviteAsync(request, createdByUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.PrimaryPhone, result.PrimaryPhone);
        Assert.Equal("Owner", result.ResidentType);
        Assert.Equal("123456", result.OtpCode);

        MockUserRepo.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        MockUserRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        MockOtpRepo.Verify(x => x.AddAsync(It.IsAny<UserOtp>()), Times.Once);
        MockInviteRepo.Verify(x => x.CreateAsync(It.IsAny<UserInvite>()), Times.Once);
    }

    [Fact]
    public async Task CreateInviteAsync_WithValidTenantData_CreatesInviteSuccessfully()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "Jane Smith",
            PrimaryPhone = "9876543211",
            ResidentType = (int)ResidentType.Tenant
        };
        var createdByUserId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "Tenant" };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync("Tenant"))
            .ReturnsAsync(role);
        MockOtpService.Setup(x => x.GenerateOtp())
            .Returns("654321");

        // Act
        var result = await OnboardingService.CreateInviteAsync(request, createdByUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tenant", result.ResidentType);
        Assert.Equal("654321", result.OtpCode);
    }

    [Fact]
    public async Task CreateInviteAsync_WithValidStaffData_CreatesInviteSuccessfully()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "Security Guard",
            PrimaryPhone = "9876543212",
            ResidentType = (int)ResidentType.Staff
        };
        var createdByUserId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var role = new Role { Id = roleId, Name = "Staff" };

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync("Staff"))
            .ReturnsAsync(role);
        MockOtpService.Setup(x => x.GenerateOtp())
            .Returns("111222");

        // Act
        var result = await OnboardingService.CreateInviteAsync(request, createdByUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Staff", result.ResidentType);
    }

    // ✅ FIXED: This test should now pass
    [Fact]
    public async Task CreateInviteAsync_WithExistingPhone_ThrowsException()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "John Doe",
            PrimaryPhone = "9876543210",
            ResidentType = (int)ResidentType.Owner
        };
        var createdByUserId = Guid.NewGuid();

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CreateInviteAsync(request, createdByUserId)
        );

        // The exact error message from ErrorMessages.PhoneAlreadyExists
        // This should match what's in your ErrorMessages constant class
        Assert.NotNull(exception.Message);
        Assert.True(
            exception.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            exception.Message.Contains("Phone", StringComparison.OrdinalIgnoreCase),
            $"Expected error message to contain 'already exists' or 'Phone', but got: {exception.Message}"
        );
    }

    [Fact]
    public async Task CreateInviteAsync_WithInvalidResidentType_ThrowsException()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "John Doe",
            PrimaryPhone = "9876543210",
            ResidentType = 999 // Invalid type
        };
        var createdByUserId = Guid.NewGuid();

        MockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        MockRoleRepo.Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CreateInviteAsync(request, createdByUserId)
        );
        Assert.Equal("Invalid resident type", exception.Message);
    }

    #endregion

    #region VerifyOtpAsync Tests

    [Fact]
    public async Task VerifyOtpAsync_WithValidOtp_UpdatesUserStatus()
    {
        // Arrange
        var request = new VerifyOtpDto
        {
            PrimaryPhone = "9876543210",
            OtpCode = "123456"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            FullName = "Test User",
            IsOtpVerified = false,
            Status = ResidentStatus.PendingOtpVerification
        };

        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            InviteStatus = InviteStatus.Pending
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        MockOtpService.Setup(x => x.ValidateOtpAsync(request.PrimaryPhone, request.OtpCode))
            .ReturnsAsync(true);
        MockInviteRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(invite);

        // Act
        var result = await OnboardingService.VerifyOtpAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.FullName, result.FullName);

        MockUserRepo.Verify(x => x.UpdateAsync(It.Is<User>(u =>
            u.IsOtpVerified == true &&
            u.Status == ResidentStatus.PendingRegistrationCompletion
        )), Times.Once);
        MockInviteRepo.Verify(x => x.UpdateStatusAsync(invite.Id, InviteStatus.OtpVerified), Times.Once);
    }

    [Fact]
    public async Task VerifyOtpAsync_WithNonExistentUser_ThrowsException()
    {
        // Arrange
        var request = new VerifyOtpDto
        {
            PrimaryPhone = "9876543210",
            OtpCode = "123456"
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.VerifyOtpAsync(request)
        );
        Assert.Contains("not found", exception.Message.ToLower());
    }

    [Fact]
    public async Task VerifyOtpAsync_WithInvalidOtp_ThrowsException()
    {
        // Arrange
        var request = new VerifyOtpDto
        {
            PrimaryPhone = "9876543210",
            OtpCode = "999999"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            FullName = "Test User"
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        MockOtpService.Setup(x => x.ValidateOtpAsync(request.PrimaryPhone, request.OtpCode))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.VerifyOtpAsync(request)
        );
        Assert.Contains("invalid", exception.Message.ToLower());
    }

    #endregion

    #region CompleteRegistrationAsync Tests

    [Fact]
    public async Task CompleteRegistrationAsync_WithValidData_CompletesRegistration()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            FullName = "John Doe Updated",
            SecondaryPhone = "9876543211",
            Email = "john@example.com",
            Username = "johndoe",
            Password = "Password@123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            FullName = "John Doe",
            IsOtpVerified = true,
            Status = ResidentStatus.PendingRegistrationCompletion,
            ResidentType = ResidentType.Owner
        };

        var invite = new UserInvite
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            InviteStatus = InviteStatus.OtpVerified
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        MockUserRepo.Setup(x => x.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        MockInviteRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(invite);

        // Act
        var result = await OnboardingService.CompleteRegistrationAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(request.Username, result.Username);
        Assert.Equal("PendingFlatAllocation", result.Status);

        MockUserRepo.Verify(x => x.UpdateAsync(It.Is<User>(u =>
            u.Username == request.Username &&
            u.Email == request.Email &&
            u.IsRegistrationCompleted == true &&
            u.Status == ResidentStatus.PendingFlatAllocation
        )), Times.Once);
        MockInviteRepo.Verify(x => x.UpdateStatusAsync(invite.Id, InviteStatus.Completed), Times.Once);
        MockEmailService.Verify(x => x.SendRegistrationCompletedToAdminAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
        ), Times.Once);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_WithNonExistentUser_ThrowsException()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            Username = "johndoe",
            Password = "Password@123"
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CompleteRegistrationAsync(request)
        );
        Assert.Contains("not found", exception.Message.ToLower());
    }

    [Fact]
    public async Task CompleteRegistrationAsync_WithoutOtpVerification_ThrowsException()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            Username = "johndoe",
            Password = "Password@123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            IsOtpVerified = false
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CompleteRegistrationAsync(request)
        );
        Assert.Contains("verify OTP", exception.Message);
    }

    // ✅ FIXED: This test should now pass
    [Fact]
    public async Task CompleteRegistrationAsync_WithExistingUsername_ThrowsException()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            FullName = "John Doe",
            Username = "existinguser",
            Password = "Password@123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            IsOtpVerified = true,
            Status = ResidentStatus.PendingRegistrationCompletion
        };

        MockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        MockUserRepo.Setup(x => x.UsernameExistsAsync(request.Username))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => OnboardingService.CompleteRegistrationAsync(request)
        );

        // The exact error message from ErrorMessages.UsernameAlreadyExists
        Assert.NotNull(exception.Message);
        Assert.True(
            exception.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            exception.Message.Contains("Username", StringComparison.OrdinalIgnoreCase),
            $"Expected error message to contain 'already exists' or 'Username', but got: {exception.Message}"
        );
    }

    #endregion
}
*/

















