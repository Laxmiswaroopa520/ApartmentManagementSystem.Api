// ApartmentManagementSystem.UnitTests/Services/AuthServiceTests.cs
// CORRECTED VERSION - Added BCrypt.Net using directive
using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Tests.Common.Builders;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using BCrypt.Net; // BCrypt namespace

namespace ApartmentManagementSystem.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup JWT configuration
            _mockConfiguration.Setup(c => c["JwtSettings:SecretKey"])
                .Returns("ThisIsAVerySecretKeyForTestingPurposesOnly12345");
            _mockConfiguration.Setup(c => c["JwtSettings:Issuer"])
                .Returns("TestIssuer");
            _mockConfiguration.Setup(c => c["JwtSettings:Audience"])
                .Returns("TestAudience");

            _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var password = "Test@123";
            var user = TestDataBuilder.CreateTestUser(
                fullName: "John Doe",
                phone: "1234567890",
                email: "john@example.com"
            );
            user.PasswordHash = BCrypt.HashPassword(password); // ✅ CORRECTED
            user.Username = "johndoe";
            user.IsActive = true;

            var role = TestDataBuilder.CreateTestRole("SuperAdmin");
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                Role = role
            });

            var loginRequest = new LoginRequestDto
            {
                Username = "johndoe",
                Password = password
            };

            _mockUserRepository
                .Setup(r => r.GetByUsernameWithRolesAsync(loginRequest.Username))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.UserId.Should().Be(user.Id);
            result.FullName.Should().Be(user.FullName);
            result.Role.Should().Be("SuperAdmin");
        }

        [Fact]
        public async Task LoginAsync_WithInvalidUsername_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "Test@123"
            };

            _mockUserRepository
                .Setup(r => r.GetByUsernameWithRolesAsync(loginRequest.Username))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginRequest)
            );
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = TestDataBuilder.CreateTestUser();
            user.PasswordHash = BCrypt.HashPassword("CorrectPassword");
            user.Username = "testuser";

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            _mockUserRepository
                .Setup(r => r.GetByUsernameWithRolesAsync(loginRequest.Username))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginRequest)
            );
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ThrowsUnauthorizedException()
        {
            // Arrange
            var password = "Test@123";
            var user = TestDataBuilder.CreateTestUser(isActive: false);
            user.PasswordHash = BCrypt.HashPassword(password);
            user.Username = "inactiveuser";

            var loginRequest = new LoginRequestDto
            {
                Username = "inactiveuser",
                Password = password
            };

            _mockUserRepository
                .Setup(r => r.GetByUsernameWithRolesAsync(loginRequest.Username))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginRequest)
            );

            exception.Message.Should().Contain("inactive");
        }

        [Fact]
        public async Task IsUserActiveAsync_WithActiveUser_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;
            user.IsActive = true;

            _mockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.IsUserActiveAsync(userId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsUserActiveAsync_WithInactiveUser_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = TestDataBuilder.CreateTestUser(isActive: false);
            user.Id = userId;

            _mockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.IsUserActiveAsync(userId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsUserActiveAsync_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.IsUserActiveAsync(userId);

            // Assert
            result.Should().BeFalse();
        }
    }
}