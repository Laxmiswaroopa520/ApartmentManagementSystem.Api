// ApartmentManagementSystem.IntegrationTests/Controllers/CommunityMembersApiControllerTests.cs
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.IntegrationTests.Infrastructure;
using ApartmentManagementSystem.Tests.Common.Builders;
using ApartmentManagementSystem.Tests.Common.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace ApartmentManagementSystem.IntegrationTests.Controllers
{
    public class CommunityMembersApiControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public CommunityMembersApiControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<string> GetAuthToken(string roleName = "SuperAdmin")
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var role = TestDataBuilder.CreateTestRole(roleName);
            var user = TestDataBuilder.CreateTestUser();
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

            return TestHelpers.GenerateJwtToken(user.Id, user.FullName, roleName);
        }

        [Fact]
        public async Task GetAllCommunityMembers_WithAuth_ReturnsOk()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/CommunityMembers");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("success");
        }

        [Fact]
        public async Task GetAllCommunityMembers_WithoutAuth_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/CommunityMembers");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAllCommunityMembers_WithApartmentId_ReturnsFilteredMembers()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create test apartment and community member
            var apartment = TestDataBuilder.CreateTestApartment();
            var user = TestDataBuilder.CreateTestUser("President User");
            var communityMember = TestDataBuilder.CreateTestCommunityMember(
                user.Id, apartment.Id, "President");

            context.Apartments.Add(apartment);
            context.Users.Add(user);
            context.Set<CommunityMember>().Add(communityMember);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/CommunityMembers?apartmentId={apartment.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CommunityMemberDto>>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetEligibleResidents_WithValidApartmentId_ReturnsResidents()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create apartment with resident owner
            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);
            var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, "101");

            var role = TestDataBuilder.CreateTestRole("ResidentOwner");
            var user = TestDataBuilder.CreateTestUser("Owner User");
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id, Role = role });

            var mapping = new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FlatId = flat.Id,
                IsActive = true
            };

            if (!context.Roles.Any(r => r.Name == role.Name))
            {
                context.Roles.Add(role);
            }

            context.Apartments.Add(apartment);
            context.Floors.Add(floor);
            context.Flats.Add(flat);
            context.Users.Add(user);
            context.Set<UserFlatMapping>().Add(mapping);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/CommunityMembers/eligible-residents/{apartment.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AssignCommunityRole_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create apartment and resident
            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);
            var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, "101");

            var role = TestDataBuilder.CreateTestRole("ResidentOwner");
            var user = TestDataBuilder.CreateTestUser("Resident User");
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id, Role = role });

            var mapping = new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FlatId = flat.Id,
                IsActive = true
            };

            if (!context.Roles.Any(r => r.Name == role.Name))
            {
                context.Roles.Add(role);
            }

            context.Apartments.Add(apartment);
            context.Floors.Add(floor);
            context.Flats.Add(flat);
            context.Users.Add(user);
            context.Set<UserFlatMapping>().Add(mapping);
            await context.SaveChangesAsync();

            var request = new AssignCommunityRoleRequestDto
            {
                UserId = user.Id,
                CommunityRole = "President",
                ApartmentId = apartment.Id
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/CommunityMembers/assign-role", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CommunityMemberDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data!.Role.Should().Be("President");
        }

        [Fact]
        public async Task AssignCommunityRole_WithoutApartmentId_ReturnsBadRequest()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new AssignCommunityRoleRequestDto
            {
                UserId = Guid.NewGuid(),
                CommunityRole = "President",
                ApartmentId = null // Missing apartment ID
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/CommunityMembers/assign-role", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RemoveCommunityRole_WithValidUserId_ReturnsSuccess()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create community member
            var apartment = TestDataBuilder.CreateTestApartment();
            var user = TestDataBuilder.CreateTestUser();
            var communityMember = TestDataBuilder.CreateTestCommunityMember(
                user.Id, apartment.Id, "Secretary");

            context.Apartments.Add(apartment);
            context.Users.Add(user);
            context.Set<CommunityMember>().Add(communityMember);
            await context.SaveChangesAsync();

            var request = new RemoveCommunityRoleRequestDto
            {
                UserId = user.Id
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/CommunityMembers/remove-role", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
            apiResponse.Data.Should().BeTrue();
        }

        [Theory]
        [InlineData("President")]
        [InlineData("Secretary")]
        [InlineData("Treasurer")]
        public async Task AssignCommunityRole_WithDifferentRoles_AssignsCorrectly(string roleName)
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create unique apartment for each test to avoid conflicts
            var apartment = TestDataBuilder.CreateTestApartment($"Building_{Guid.NewGuid():N}");
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);
            var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, "101");

            var userRole = TestDataBuilder.CreateTestRole("ResidentOwner");
            var user = TestDataBuilder.CreateTestUser($"User_{Guid.NewGuid():N}");
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id, Role = userRole });

            var mapping = new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FlatId = flat.Id,
                IsActive = true
            };

            if (!context.Roles.Any(r => r.Name == userRole.Name))
            {
                context.Roles.Add(userRole);
            }

            context.Apartments.Add(apartment);
            context.Floors.Add(floor);
            context.Flats.Add(flat);
            context.Users.Add(user);
            context.Set<UserFlatMapping>().Add(mapping);
            await context.SaveChangesAsync();

            var request = new AssignCommunityRoleRequestDto
            {
                UserId = user.Id,
                CommunityRole = roleName,
                ApartmentId = apartment.Id
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/CommunityMembers/assign-role", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CommunityMemberDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            apiResponse!.Data!.Role.Should().Be(roleName);
        }

        [Fact]
        public async Task GetAllCommunityMembers_WithManagerRole_ReturnsOk()
        {
            // Arrange
            var token = await GetAuthToken("Manager");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/CommunityMembers");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAllCommunityMembers_WithInvalidRole_ReturnsForbidden()
        {
            // Arrange
            var token = await GetAuthToken("Tenant");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/CommunityMembers");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}