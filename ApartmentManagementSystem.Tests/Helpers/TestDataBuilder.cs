using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Tests.Helpers;

/// <summary>
/// Provides test data builders for creating test entities
/// </summary>
public static class TestDataBuilder
{
    public static User CreateTestUser(
        string username = "testuser",
        string password = "Password@123",
        string fullName = "Test User",
        bool isActive = true,
        ResidentType? residentType = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName,
            PrimaryPhone = "9876543210",
            Email = $"{username}@test.com",
            IsActive = isActive,
            IsOtpVerified = true,
            IsRegistrationCompleted = true,
            ResidentType = residentType,
            Status = ResidentStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>()
        };
    }

    public static Role CreateTestRole(string roleName)
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>()
        };
    }

    public static UserRole CreateUserRole(Guid userId, Guid roleId)
    {
        return new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTime.UtcNow
        };
    }

    public static UserOtp CreateTestOtp(string phoneNumber, string otpCode)
    {
        return new UserOtp
        {
            Id = Guid.NewGuid(),
            PhoneNumber = phoneNumber,
            OtpCode = otpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static UserInvite CreateTestInvite(
        string fullName,
        string primaryPhone,
        Guid roleId,
        ResidentType residentType)
    {
        return new UserInvite
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            PrimaryPhone = primaryPhone,
            RoleId = roleId,
            ResidentType = residentType,
            InviteStatus = InviteStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = Guid.NewGuid()
        };
    }
}

/// <summary>
/// Contains constant values used across tests
/// </summary>
public static class TestConstants
{
    public const string ValidPassword = "Password@123";
    public const string AdminUsername = "testadmin";
    public const string OwnerUsername = "testowner";
    public const string InactiveUsername = "inactiveuser";

    public const string ValidPhoneNumber = "9876543210";
    public const string ValidEmail = "test@example.com";

    public static class RoleIds
    {
        public static readonly Guid SuperAdmin = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Manager = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid ResidentOwner = Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid Tenant = Guid.Parse("10000000-0000-0000-0000-000000000004");
        public static readonly Guid Staff = Guid.Parse("10000000-0000-0000-0000-000000000005");
    }

    public static class UserIds
    {
        public static readonly Guid Admin = Guid.Parse("20000000-0000-0000-0000-000000000001");
        public static readonly Guid Owner = Guid.Parse("20000000-0000-0000-0000-000000000002");
        public static readonly Guid Inactive = Guid.Parse("20000000-0000-0000-0000-000000000003");
    }
}

/// <summary>
/// Assertion helpers for common test scenarios
/// </summary>
public static class TestAssertions
{
    public static void AssertUserEquals(User expected, User actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Username, actual.Username);
        Assert.Equal(expected.FullName, actual.FullName);
        Assert.Equal(expected.Email, actual.Email);
        Assert.Equal(expected.PrimaryPhone, actual.PrimaryPhone);
        Assert.Equal(expected.IsActive, actual.IsActive);
    }

    public static void AssertValidJwtToken(string token)
    {
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token); // JWT has parts separated by dots
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length); // Header, Payload, Signature
    }

    public static void AssertApiResponseSuccess<T>(T? response) where T : class
    {
        Assert.NotNull(response);
    }
}