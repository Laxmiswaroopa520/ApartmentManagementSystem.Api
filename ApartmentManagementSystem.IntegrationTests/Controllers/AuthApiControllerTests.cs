// ApartmentManagementSystem.IntegrationTests/Controllers/AuthApiControllerTests.cs
using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.IntegrationTests.Infrastructure;
using ApartmentManagementSystem.Tests.Common.Builders;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ApartmentManagementSystem.IntegrationTests.Controllers
{
    public class AuthApiControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient Httpclient;
        private readonly CustomWebApplicationFactory<Program> Factory;

        public AuthApiControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            Factory = factory;
            Httpclient = factory.CreateClient();
        }

        private async Task SeedTestUser(User user, Role role)
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!context.Roles.Any(r => r.Name == role.Name))
            {
                context.Roles.Add(role);
            }

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                Role = role
            });

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var password = "Test@123";
            var role = TestDataBuilder.CreateTestRole("SuperAdmin");
            var user = TestDataBuilder.CreateTestUser(
                fullName: "Test User",
                phone: "1234567890",
                email: "test@example.com"
            );
            user.Username = "testuser";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.IsActive = true;

            await SeedTestUser(user, role);

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = password
            };

            // Act
            var response = await Httpclient.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data!.Token.Should().NotBeNullOrEmpty();
            apiResponse.Data.UserId.Should().Be(user.Id);
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "Test@123"
            };

            // Act
            var response = await Httpclient.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var password = "CorrectPassword";
            var role = TestDataBuilder.CreateTestRole("Manager");
            var user = TestDataBuilder.CreateTestUser();
            user.Username = "testuser2";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.IsActive = true;

            await SeedTestUser(user, role);

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser2",
                Password = "WrongPassword"
            };

            // Act
            var response = await Httpclient.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithInactiveUser_ReturnsUnauthorizedWithMessage()
        {
            // Arrange
            var password = "Test@123";
            var role = TestDataBuilder.CreateTestRole("ResidentOwner");
            var user = TestDataBuilder.CreateTestUser(isActive: false);
            user.Username = "inactiveuser";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            await SeedTestUser(user, role);

            var loginRequest = new LoginRequestDto
            {
                Username = "inactiveuser",
                Password = password
            };

            // Act
            var response = await Httpclient.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("inactive");
        }

        [Fact]
        public async Task Login_WithEmptyCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "",
                Password = ""
            };

            // Act
            var response = await Httpclient.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}