using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.Tests.Services;

/// <summary>
/// Additional edge case and security tests for AuthService
/// </summary>
public class AuthServiceEdgeCaseTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthService _authService;

    public AuthServiceEdgeCaseTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockConfig = new Mock<IConfiguration>();

        _mockConfig.Setup(x => x["JwtSettings:SecretKey"])
            .Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm!");
        _mockConfig.Setup(x => x["JwtSettings:Issuer"]).Returns("ApartmentManagementSystem");
        _mockConfig.Setup(x => x["JwtSettings:Audience"]).Returns("ApartmentManagementSystemUsers");

        _authService = new AuthService(_mockUserRepo.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task LoginAsync_WithNullUsername_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = null!,
            Password = "Password@123"
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_WithCaseInsensitiveUsername_HandlesCorrectly()
    {
        // Arrange - Username stored as lowercase
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
                new UserRole { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("TESTUSER"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "TESTUSER", // Uppercase
            Password = "Password@123"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_WithSpecialCharactersInPassword_HandlesCorrectly()
    {
        // Arrange
        var specialPassword = "P@ssw0rd!#$%^&*()";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(specialPassword);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = specialPassword
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_WithVeryLongPassword_HandlesCorrectly()
    {
        // Arrange
        var longPassword = new string('a', 100) + "@123A";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(longPassword);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = longPassword
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LoginAsync_WithUserHavingNoRoles_ReturnsNullRole()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password@123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>() // No roles
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Role);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task LoginAsync_WithWhitespacePassword_ThrowsUnauthorizedException(string password)
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = password
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("RealPassword@123"),
            IsActive = true,
            UserRoles = new List<UserRole>()
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task LoginAsync_WithPasswordContainingNullBytes_HandlesCorrectly()
    {
        // Arrange
        var password = "Pass\0word@123";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = hashedPassword,
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = password
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task IsUserActiveAsync_WithNullUserId_ReturnsFalse()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        _mockUserRepo.Setup(x => x.GetByIdAsync(emptyGuid))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.IsUserActiveAsync(emptyGuid);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _authService.LoginAsync(request)
        );
        Assert.Equal("Database connection failed", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_TokenContainsAllExpectedClaims()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password@123");
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
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

        _mockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert - Verify token structure
        Assert.NotNull(result.Token);
        var parts = result.Token.Split('.');
        Assert.Equal(3, parts.Length); // Header, Payload, Signature

        // Verify response contains correct data
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Test User", result.FullName);
        Assert.Equal("ResidentOwner", result.Role); // First role
    }
}