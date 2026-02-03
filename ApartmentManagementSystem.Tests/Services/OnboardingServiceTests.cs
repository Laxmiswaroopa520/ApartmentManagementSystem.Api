using ApartmentManagementSystem.Application.DTOs.Onboarding;
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
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IUserInviteRepository> _mockInviteRepo;
    private readonly Mock<IUserOtpRepository> _mockOtpRepo;
    private readonly Mock<IOtpService> _mockOtpService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IRoleRepository> _mockRoleRepo;
    private readonly OnboardingService _onboardingService;

    public OnboardingServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockInviteRepo = new Mock<IUserInviteRepository>();
        _mockOtpRepo = new Mock<IUserOtpRepository>();
        _mockOtpService = new Mock<IOtpService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockRoleRepo = new Mock<IRoleRepository>();

        _onboardingService = new OnboardingService(
            _mockUserRepo.Object,
            _mockInviteRepo.Object,
            _mockOtpRepo.Object,
            _mockOtpService.Object,
            _mockEmailService.Object,
            _mockRoleRepo.Object
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

        _mockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        _mockRoleRepo.Setup(x => x.GetByNameAsync("ResidentOwner"))
            .ReturnsAsync(role);
        _mockOtpService.Setup(x => x.GenerateOtp())
            .Returns("123456");

        // Act
        var result = await _onboardingService.CreateInviteAsync(request, createdByUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.PrimaryPhone, result.PrimaryPhone);
        Assert.Equal("Owner", result.ResidentType);
        Assert.Equal("123456", result.OtpCode);

        _mockUserRepo.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _mockUserRepo.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockOtpRepo.Verify(x => x.AddAsync(It.IsAny<UserOtp>()), Times.Once);
        _mockInviteRepo.Verify(x => x.CreateAsync(It.IsAny<UserInvite>()), Times.Once);
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

        _mockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        _mockRoleRepo.Setup(x => x.GetByNameAsync("Tenant"))
            .ReturnsAsync(role);
        _mockOtpService.Setup(x => x.GenerateOtp())
            .Returns("654321");

        // Act
        var result = await _onboardingService.CreateInviteAsync(request, createdByUserId);

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

        _mockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        _mockRoleRepo.Setup(x => x.GetByNameAsync("Staff"))
            .ReturnsAsync(role);
        _mockOtpService.Setup(x => x.GenerateOtp())
            .Returns("111222");

        // Act
        var result = await _onboardingService.CreateInviteAsync(request, createdByUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Staff", result.ResidentType);
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
        var createdByUserId = Guid.NewGuid();

        _mockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _onboardingService.CreateInviteAsync(request, createdByUserId)
        );
        Assert.Contains("already exists", exception.Message.ToLower());
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

        _mockUserRepo.Setup(x => x.PhoneExistsAsync(request.PrimaryPhone))
            .ReturnsAsync(false);
        _mockRoleRepo.Setup(x => x.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _onboardingService.CreateInviteAsync(request, createdByUserId)
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

        _mockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        _mockOtpService.Setup(x => x.ValidateOtpAsync(request.PrimaryPhone, request.OtpCode))
            .ReturnsAsync(true);
        _mockInviteRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(invite);

        // Act
        var result = await _onboardingService.VerifyOtpAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.FullName, result.FullName);

        _mockUserRepo.Verify(x => x.UpdateAsync(It.Is<User>(u =>
            u.IsOtpVerified == true &&
            u.Status == ResidentStatus.PendingRegistrationCompletion
        )), Times.Once);
        _mockInviteRepo.Verify(x => x.UpdateStatusAsync(invite.Id, InviteStatus.OtpVerified), Times.Once);
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

        _mockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _onboardingService.VerifyOtpAsync(request)
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

        _mockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        _mockOtpService.Setup(x => x.ValidateOtpAsync(request.PrimaryPhone, request.OtpCode))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _onboardingService.VerifyOtpAsync(request)
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

        _mockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        _mockUserRepo.Setup(x => x.UsernameExistsAsync(request.Username))
            .ReturnsAsync(false);
        _mockInviteRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(invite);

        // Act
        var result = await _onboardingService.CompleteRegistrationAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(request.Username, result.Username);
        Assert.Equal("PendingFlatAllocation", result.Status);

        _mockUserRepo.Verify(x => x.UpdateAsync(It.Is<User>(u =>
            u.Username == request.Username &&
            u.Email == request.Email &&
            u.IsRegistrationCompleted == true &&
            u.Status == ResidentStatus.PendingFlatAllocation
        )), Times.Once);
        _mockInviteRepo.Verify(x => x.UpdateStatusAsync(invite.Id, InviteStatus.Completed), Times.Once);
        _mockEmailService.Verify(x => x.SendRegistrationCompletedToAdminAsync(
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

        _mockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _onboardingService.CompleteRegistrationAsync(request)
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

        _mockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _onboardingService.CompleteRegistrationAsync(request)
        );
        Assert.Contains("verify OTP", exception.Message);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_WithExistingUsername_ThrowsException()
    {
        // Arrange
        var request = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543210",
            Username = "existinguser",
            Password = "Password@123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            PrimaryPhone = request.PrimaryPhone,
            IsOtpVerified = true
        };

        _mockUserRepo.Setup(x => x.GetByPhoneAsync(request.PrimaryPhone))
            .ReturnsAsync(user);
        _mockUserRepo.Setup(x => x.UsernameExistsAsync(request.Username))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _onboardingService.CompleteRegistrationAsync(request)
        );
        Assert.Contains("already exists", exception.Message.ToLower());
    }

    #endregion
}