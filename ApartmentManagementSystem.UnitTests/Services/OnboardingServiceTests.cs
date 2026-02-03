// ApartmentManagementSystem.UnitTests/Services/OnboardingServiceTests.cs
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Tests.Common.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.UnitTests.Services
{
    public class OnboardingServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserInviteRepository> _mockInviteRepository;
        private readonly Mock<IUserOtpRepository> _mockOtpRepository;
        private readonly Mock<IOtpService> _mockOtpService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IRoleRepository> _mockRoleRepository;
        private readonly OnboardingService _service;

        public OnboardingServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockInviteRepository = new Mock<IUserInviteRepository>();
            _mockOtpRepository = new Mock<IUserOtpRepository>();
            _mockOtpService = new Mock<IOtpService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockRoleRepository = new Mock<IRoleRepository>();

            _service = new OnboardingService(
                _mockUserRepository.Object,
                _mockInviteRepository.Object,
                _mockOtpRepository.Object,
                _mockOtpService.Object,
                _mockEmailService.Object,
                _mockRoleRepository.Object
            );
        }

        [Fact]
        public async Task CreateInviteAsync_WithValidData_CreatesInvite()
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var request = new CreateUserInviteDto
            {
                FullName = "John Doe",
                PrimaryPhone = "1234567890",
                ResidentType = (int)ResidentType.Owner
            };

            var role = TestDataBuilder.CreateTestRole("ResidentOwner");
            var otpCode = "123456";

            _mockUserRepository
                .Setup(r => r.PhoneExistsAsync(request.PrimaryPhone))
                .ReturnsAsync(false);

            _mockRoleRepository
                .Setup(r => r.GetByNameAsync("ResidentOwner"))
                .ReturnsAsync(role);

            _mockOtpService
                .Setup(s => s.GenerateOtp())
                .Returns(otpCode);

            _mockUserRepository
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockUserRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mockOtpRepository
                .Setup(r => r.AddAsync(It.IsAny<UserOtp>()))
                .Returns(Task.CompletedTask);

            _mockInviteRepository
                .Setup(r => r.CreateAsync(It.IsAny<UserInvite>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateInviteAsync(request, createdBy);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(request.FullName);
            result.PrimaryPhone.Should().Be(request.PrimaryPhone);
            result.OtpCode.Should().Be(otpCode);

            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _mockOtpRepository.Verify(r => r.AddAsync(It.IsAny<UserOtp>()), Times.Once);
            _mockInviteRepository.Verify(r => r.CreateAsync(It.IsAny<UserInvite>()), Times.Once);
        }

        [Fact]
        public async Task CreateInviteAsync_WithExistingPhone_ThrowsException()
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var request = new CreateUserInviteDto
            {
                FullName = "John Doe",
                PrimaryPhone = "1234567890",
                ResidentType = (int)ResidentType.Owner
            };

            _mockUserRepository
                .Setup(r => r.PhoneExistsAsync(request.PrimaryPhone))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.CreateInviteAsync(request, createdBy)
            );
        }

        [Theory]
        [InlineData(1, "ResidentOwner")] // Owner
        [InlineData(2, "Tenant")] // Tenant
        [InlineData(3, "Staff")] // Staff
        public async Task CreateInviteAsync_WithDifferentResidentTypes_CreatesCorrectRole(
            int residentType, string expectedRoleName)
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var request = new CreateUserInviteDto
            {
                FullName = "Test User",
                PrimaryPhone = "9876543210",
                ResidentType = residentType
            };

            var role = TestDataBuilder.CreateTestRole(expectedRoleName);
            var otpCode = "123456";

            _mockUserRepository
                .Setup(r => r.PhoneExistsAsync(request.PrimaryPhone))
                .ReturnsAsync(false);

            _mockRoleRepository
                .Setup(r => r.GetByNameAsync(expectedRoleName))
                .ReturnsAsync(role);

            _mockOtpService
                .Setup(s => s.GenerateOtp())
                .Returns(otpCode);

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockUserRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockOtpRepository.Setup(r => r.AddAsync(It.IsAny<UserOtp>())).Returns(Task.CompletedTask);
            _mockInviteRepository.Setup(r => r.CreateAsync(It.IsAny<UserInvite>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateInviteAsync(request, createdBy);

            // Assert
            result.Should().NotBeNull();
            _mockRoleRepository.Verify(r => r.GetByNameAsync(expectedRoleName), Times.Once);
        }

        [Fact]
        public async Task VerifyOtpAsync_WithValidOtp_VerifiesUser()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            user.IsOtpVerified = false;
            user.Status = ResidentStatus.PendingOtpVerification;

            var request = new VerifyOtpDto
            {
                PrimaryPhone = user.PrimaryPhone,
                OtpCode = "123456"
            };

            var invite = new UserInvite
            {
                Id = Guid.NewGuid(),
                PrimaryPhone = user.PrimaryPhone,
                InviteStatus = InviteStatus.Pending
            };

            _mockUserRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync(user);

            _mockOtpService
                .Setup(s => s.ValidateOtpAsync(user.PrimaryPhone, request.OtpCode))
                .ReturnsAsync(true);

            _mockUserRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockInviteRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync(invite);

            _mockInviteRepository
                .Setup(r => r.UpdateStatusAsync(invite.Id, InviteStatus.OtpVerified))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.VerifyOtpAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.UserId.Should().Be(user.Id);

            _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.IsOtpVerified == true &&
                u.Status == ResidentStatus.PendingRegistrationCompletion
            )), Times.Once);
        }

        [Fact]
        public async Task VerifyOtpAsync_WithInvalidOtp_ThrowsException()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            var request = new VerifyOtpDto
            {
                PrimaryPhone = user.PrimaryPhone,
                OtpCode = "wrong"
            };

            _mockUserRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync(user);

            _mockOtpService
                .Setup(s => s.ValidateOtpAsync(user.PrimaryPhone, request.OtpCode))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.VerifyOtpAsync(request)
            );
        }

        [Fact]
        public async Task VerifyOtpAsync_WithNonExistentUser_ThrowsException()
        {
            // Arrange
            var request = new VerifyOtpDto
            {
                PrimaryPhone = "9999999999",
                OtpCode = "123456"
            };

            _mockUserRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.VerifyOtpAsync(request)
            );
        }

        [Fact]
        public async Task CompleteRegistrationAsync_WithValidData_CompletesRegistration()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            user.IsOtpVerified = true;
            user.Status = ResidentStatus.PendingRegistrationCompletion;

            var request = new CompleteRegistrationDto
            {
                PrimaryPhone = user.PrimaryPhone,
                FullName = "Updated Name",
                SecondaryPhone = "0987654321",
                Email = "newemail@test.com",
                Username = "newusername",
                Password = "NewPassword@123"
            };

            var invite = new UserInvite
            {
                Id = Guid.NewGuid(),
                PrimaryPhone = user.PrimaryPhone,
                InviteStatus = InviteStatus.OtpVerified
            };

            _mockUserRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(r => r.UsernameExistsAsync(request.Username))
                .ReturnsAsync(false);

            _mockUserRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockInviteRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync(invite);

            _mockInviteRepository
                .Setup(r => r.UpdateStatusAsync(invite.Id, InviteStatus.Completed))
                .Returns(Task.CompletedTask);

            _mockEmailService
                .Setup(s => s.SendRegistrationCompletedToAdminAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CompleteRegistrationAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(user.Id);
            result.Username.Should().Be(request.Username);
            result.Status.Should().Be("PendingFlatAllocation");

            _mockEmailService.Verify(s => s.SendRegistrationCompletedToAdminAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()
            ), Times.Once);
        }

        [Fact]
        public async Task CompleteRegistrationAsync_WithExistingUsername_ThrowsException()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            user.IsOtpVerified = true;

            var request = new CompleteRegistrationDto
            {
                PrimaryPhone = user.PrimaryPhone,
                Username = "existinguser",
                Password = "Password@123"
            };

            _mockUserRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(r => r.UsernameExistsAsync(request.Username))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.CompleteRegistrationAsync(request)
            );
        }

        [Fact]
        public async Task CompleteRegistrationAsync_WithUnverifiedOtp_ThrowsException()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            user.IsOtpVerified = false;

            var request = new CompleteRegistrationDto
            {
                PrimaryPhone = user.PrimaryPhone,
                Username = "newuser",
                Password = "Password@123"
            };

            _mockUserRepository
                .Setup(r => r.GetByPhoneAsync(request.PrimaryPhone))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.CompleteRegistrationAsync(request)
            );

            exception.Message.Should().Contain("verify OTP");
        }
    }
}