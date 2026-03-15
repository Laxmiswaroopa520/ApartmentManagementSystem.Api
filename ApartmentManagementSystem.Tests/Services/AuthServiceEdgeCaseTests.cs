using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.Tests.Services;

public class AuthServiceEdgeCaseTests
{
    private readonly Mock<IUnitOfWork> MockUoW;
    private readonly Mock<IUserRepository> MockUserRepo;
    private readonly Mock<IConfiguration> MockConfig;
    private readonly AuthService AuthService;

    public AuthServiceEdgeCaseTests()
    {
        MockUoW = new Mock<IUnitOfWork>();
        MockUserRepo = new Mock<IUserRepository>();
        MockConfig = new Mock<IConfiguration>();

        // Wire repo mock to UoW property
        MockUoW.Setup(u => u.Users).Returns(MockUserRepo.Object);

        MockConfig.Setup(x => x["JwtSettings:SecretKey"])
            .Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm!");
        MockConfig.Setup(x => x["JwtSettings:Issuer"]).Returns("ApartmentManagementSystem");
        MockConfig.Setup(x => x["JwtSettings:Audience"]).Returns("ApartmentManagementSystemUsers");

        AuthService = new AuthService(MockUoW.Object, MockConfig.Object);
    }

    // ── Edge cases ─────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_WithNullUsername_ThrowsUnauthorizedException()
    {
        var request = new LoginRequestDto { Username = null!, Password = "Password@123" };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithCaseInsensitiveUsername_HandlesCorrectly()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("TESTUSER")).ReturnsAsync(user);

        var result = await AuthService.LoginAsync(
            new LoginRequestDto { Username = "TESTUSER", Password = "Password@123" });

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_WithSpecialCharactersInPassword_HandlesCorrectly()
    {
        var specialPassword = "P@ssw0rd!#$%^&*()";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(specialPassword),
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser")).ReturnsAsync(user);

        var result = await AuthService.LoginAsync(
            new LoginRequestDto { Username = "testuser", Password = specialPassword });

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task LoginAsync_WithVeryLongPassword_HandlesCorrectly()
    {
        var longPassword = new string('a', 100) + "@123A";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(longPassword),
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser")).ReturnsAsync(user);

        var result = await AuthService.LoginAsync(
            new LoginRequestDto { Username = "testuser", Password = longPassword });

        Assert.NotNull(result);
    }

    [Fact]
    public async Task LoginAsync_WithUserHavingNoRoles_ReturnsNullRole()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>()
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser")).ReturnsAsync(user);

        var result = await AuthService.LoginAsync(
            new LoginRequestDto { Username = "testuser", Password = "Password@123" });

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
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("RealPassword@123"),
            IsActive = true,
            UserRoles = new List<UserRole>()
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser")).ReturnsAsync(user);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(
                new LoginRequestDto { Username = "testuser", Password = password }));
    }

    [Fact]
    public async Task LoginAsync_WithPasswordContainingNullBytes_HandlesCorrectly()
    {
        var password = "Pass\0word@123";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "ResidentOwner" } }
            }
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser")).ReturnsAsync(user);

        var result = await AuthService.LoginAsync(
            new LoginRequestDto { Username = "testuser", Password = password });

        Assert.NotNull(result);
    }

    [Fact]
    public async Task IsUserActiveAsync_WithNullUserId_ReturnsFalse()
    {
        MockUserRepo.Setup(x => x.GetByIdAsync(Guid.Empty)).ReturnsAsync((User?)null);

        Assert.False(await AuthService.IsUserActiveAsync(Guid.Empty));
    }

    [Fact]
    public async Task LoginAsync_RepositoryThrowsException_PropagatesException()
    {
        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var exception = await Assert.ThrowsAsync<Exception>(
            () => AuthService.LoginAsync(
                new LoginRequestDto { Username = "testuser", Password = "Password@123" }));

        Assert.Equal("Database connection failed", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_TokenContainsAllExpectedClaims()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            FullName = "Test User",
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "ResidentOwner" } },
                new() { Role = new Role { Name = "President"     } }
            }
        };

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser")).ReturnsAsync(user);

        var result = await AuthService.LoginAsync(
            new LoginRequestDto { Username = "testuser", Password = "Password@123" });

        Assert.NotNull(result.Token);
        Assert.Equal(3, result.Token.Split('.').Length); // valid JWT structure

        Assert.Equal(userId, result.UserId);
        Assert.Equal("Test User", result.FullName);
        Assert.Equal("ResidentOwner", result.Role);
    }
}











/*using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.Tests.Services;

/// Additional edge case and security tests for AuthService
public class AuthServiceEdgeCaseTests
{
    private readonly Mock<IUserRepository> MockUserRepo;
    private readonly Mock<IConfiguration> MockConfig;
    private readonly AuthService AuthService;

    public AuthServiceEdgeCaseTests()
    {
        MockUserRepo = new Mock<IUserRepository>();
        MockConfig = new Mock<IConfiguration>();

        MockConfig.Setup(x => x["JwtSettings:SecretKey"])
            .Returns("YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm!");
        MockConfig.Setup(x => x["JwtSettings:Issuer"]).Returns("ApartmentManagementSystem");
        MockConfig.Setup(x => x["JwtSettings:Audience"]).Returns("ApartmentManagementSystemUsers");

        AuthService = new AuthService(MockUserRepo.Object, MockConfig.Object);
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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(request)
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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("TESTUSER"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "TESTUSER", // Uppercase
            Password = "Password@123"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = specialPassword
        };

        // Act
        var result = await AuthService.LoginAsync(request);

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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = longPassword
        };

        // Act
        var result = await AuthService.LoginAsync(request);

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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => AuthService.LoginAsync(request)
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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = password
        };

        // Act
        var result = await AuthService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task IsUserActiveAsync_WithNullUserId_ReturnsFalse()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        MockUserRepo.Setup(x => x.GetByIdAsync(emptyGuid))
            .ReturnsAsync((User?)null);

        // Act
        var result = await AuthService.IsUserActiveAsync(emptyGuid);

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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => AuthService.LoginAsync(request)
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

        MockUserRepo.Setup(x => x.GetByUsernameWithRolesAsync("testuser"))
            .ReturnsAsync(user);

        var request = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password@123"
        };

        // Act
        var result = await AuthService.LoginAsync(request);

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
*/