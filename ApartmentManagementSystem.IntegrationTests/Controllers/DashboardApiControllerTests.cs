// ApartmentManagementSystem.IntegrationTests/Controllers/DashboardApiControllerTests.cs
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.IntegrationTests.Infrastructure;
using ApartmentManagementSystem.Tests.Common.Builders;
using ApartmentManagementSystem.Tests.Common.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace ApartmentManagementSystem.IntegrationTests.Controllers
{
    public class DashboardApiControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public DashboardApiControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<(string token, Guid userId)> GetAuthTokenWithUserId(string roleName = "SuperAdmin")
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var role = TestDataBuilder.CreateTestRole(roleName);
            var user = TestDataBuilder.CreateTestUser($"User_{Guid.NewGuid():N}");
            user.Username = $"testuser_{Guid.NewGuid():N}";
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123");
            user.IsActive = true;

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

            var token = TestHelpers.GenerateJwtToken(user.Id, user.FullName, roleName);
            return (token, user.Id);
        }

        [Fact]
        public async Task GetAdminDashboard_WithSuperAdminAuth_ReturnsOk()
        {
            // Arrange
            var (token, _) = await GetAuthTokenWithUserId("SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Dashboard/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AdminDashboardDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data!.Stats.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAdminDashboard_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/Dashboard/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("President")]
        [InlineData("Secretary")]
        [InlineData("Treasurer")]
        public async Task GetAdminDashboard_WithCommunityLeaderRoles_ReturnsOk(string roleName)
        {
            // Arrange
            var (token, _) = await GetAuthTokenWithUserId(roleName);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Dashboard/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAdminDashboard_WithInvalidRole_ReturnsForbidden()
        {
            // Arrange
            var (token, _) = await GetAuthTokenWithUserId("Tenant");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Dashboard/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetOwnerDashboard_WithResidentOwnerAuth_ReturnsOk()
        {
            // Arrange
            var (token, userId) = await GetAuthTokenWithUserId("ResidentOwner");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create apartment and flat for the owner
            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);
            var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, "101");
            flat.OwnerUserId = userId;

            context.Apartments.Add(apartment);
            context.Floors.Add(floor);
            context.Flats.Add(flat);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Dashboard/owner");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<OwnerDashboardDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetOwnerDashboard_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/Dashboard/owner");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetTenantDashboard_WithTenantAuth_ReturnsOk()
        {
            // Arrange
            var (token, userId) = await GetAuthTokenWithUserId("Tenant");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create apartment, flat, and mapping for tenant
            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);
            var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, "102");

            var mapping = new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FlatId = flat.Id,
                IsActive = true
            };

            context.Apartments.Add(apartment);
            context.Floors.Add(floor);
            context.Flats.Add(flat);
            context.Set<UserFlatMapping>().Add(mapping);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Dashboard/tenant");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<TenantDashboardDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetDashboardStats_WithAuth_ReturnsStats()
        {
            // Arrange
            var (token, _) = await GetAuthTokenWithUserId("SuperAdmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create test data
            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);

            for (int i = 1; i <= 5; i++)
            {
                var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, $"10{i}");
                flat.IsOccupied = i <= 3; // 3 occupied, 2 vacant
                context.Flats.Add(flat);
            }

            context.Apartments.Add(apartment);
            context.Floors.Add(floor);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Dashboard/stats");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<DashboardStatsDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data!.TotalFlats.Should().BeGreaterOrEqualTo(5);
            apiResponse.Data.OccupiedFlats.Should().BeGreaterOrEqualTo(3);
        }

        [Fact]
        public async Task GetDashboardStats_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/Dashboard/stats");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetOwnerDashboard_WithNoFlats_ReturnsEmptyFlatsList()
        {
            // Arrange
            var (token, _) = await GetAuthTokenWithUserId("ResidentOwner");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Dashboard/owner");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<OwnerDashboardDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse!.Data!.MyFlats.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTenantDashboard_WithNoMapping_ReturnsNullFlat()
        {
            // Arrange
            var (token, _) = await GetAuthTokenWithUserId("Tenant");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Dashboard/tenant");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<TenantDashboardDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse!.Data!.MyFlat.Should().BeNull();
        }

        [Fact]
        public async Task GetAdminDashboard_ReturnsCorrectUserInfo()
        {
            // Arrange
            var (token, userId) = await GetAuthTokenWithUserId("Manager");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/Dashboard/admin");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AdminDashboardDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse!.Data.Should().NotBeNull();
            apiResponse.Data!.FullName.Should().NotBeNullOrEmpty();
            apiResponse.Data.Role.Should().Be("Manager");
        }

        [Fact]
        public async Task GetOwnerDashboard_WithMultipleFlats_ReturnsAllFlats()
        {
            // Arrange
            var (token, userId) = await GetAuthTokenWithUserId("ResidentOwner");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);

            // Create 3 flats owned by the user
            for (int i = 1; i <= 3; i++)
            {
                var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, $"10{i}");
                flat.OwnerUserId = userId;
                context.Flats.Add(flat);
            }

            context.Apartments.Add(apartment);
            context.Floors.Add(floor);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Dashboard/owner");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<OwnerDashboardDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse!.Data!.MyFlats.Should().HaveCount(3);
        }
    }
}