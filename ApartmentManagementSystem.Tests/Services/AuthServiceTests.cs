using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> MockUserRepo;
    private readonly Mock<IConfiguration> MockConfig;
    private readonly AuthService AuthService;
    private readonly string testSecretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm!";

    public AuthServiceTests()
    {
        MockUserRepo = new Mock<IUserRepository>();
        MockConfig = new Mock<IConfiguration>();

        // Setup JWT configuration
        MockConfig.Setup(x => x["JwtSettings:SecretKey"]).Returns(testSecretKey);
        MockConfig.Setup(x => x["JwtSettings:Issuer"]).Returns("ApartmentManagementSystem");
        MockConfig.Setup(x => x["JwtSettings:Audience"]).Returns("ApartmentManagementSystemUsers");

        AuthService = new AuthService(MockUserRepo.Object, MockConfig.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password@123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Name = "ResidentOwner" }
                }
            }
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(request.Username))
            .ReturnsAsync(user);

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal("ResidentOwner", result.Role);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "nonexistent",
            Password = "Password@123"
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(request.Username))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(request)
        );
        Assert.Equal("Invalid credentials", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            IsActive = true,
            UserRoles = new List<UserRole>()
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(request.Username))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(request)
        );
        Assert.Equal("Invalid credentials", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password@123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            FullName = "Test User",
            IsActive = false, // Inactive user
            UserRoles = new List<UserRole>()
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(request.Username))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(request)
        );
        Assert.Contains("inactive", exception.Message.ToLower());
    }

    [Fact]
    public async Task LoginAsync_WithMultipleRoles_ReturnsFirstRole()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password@123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = new Role { Name = "ResidentOwner" } },
                new UserRole { Role = new Role { Name = "President" } }
            }
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(request.Username))
            .ReturnsAsync(user);

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ResidentOwner", result.Role);
    }

    [Fact]
    public async Task IsUserActiveAsync_WithActiveUser_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            IsActive = true
        };

        MockUserRepo.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await AuthService.IsUserActiveAsync(userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUserActiveAsync_WithInactiveUser_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            IsActive = false
        };

        MockUserRepo.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await AuthService.IsUserActiveAsync(userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsUserActiveAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        MockUserRepo.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await AuthService.IsUserActiveAsync(userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_EmptyUsername_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "",
            Password = "Password@123"
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(request.Username))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(request)
        );
    }
}