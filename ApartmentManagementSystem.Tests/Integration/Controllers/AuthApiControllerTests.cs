using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.DTOs.Common;
using Xunit;

namespace ApartmentManagementSystem.Tests.Integration.Controllers;

public class AuthApiControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthApiControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "Admin@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.NotEmpty(apiResponse.Data.Token);
        Assert.Equal("Test Admin", apiResponse.Data.FullName);
        Assert.Equal("SuperAdmin", apiResponse.Data.Role);
    }

    [Fact]
    public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "nonexistentuser",
            Password = "SomePassword@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Invalid credentials", apiResponse.Message);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "WrongPassword@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Invalid credentials", apiResponse.Message);
    }

    [Fact]
    public async Task Login_WithInactiveUser_ReturnsUnauthorizedWithInactiveMessage()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "inactiveuser",
            Password = "Inactive@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("inactive", apiResponse.Message.ToLower());
        Assert.Equal("ACCOUNT_INACTIVE", apiResponse.ErrorCode);
    }

    [Fact]
    public async Task Login_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "",
            Password = "Password@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("required", apiResponse.Message.ToLower());
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("required", apiResponse.Message.ToLower());
    }

    [Fact]
    public async Task Login_WithWhitespaceCredentials_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "   ",
            Password = "   "
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task IsUserActive_WithValidToken_ReturnsActiveStatus()
    {
        // Arrange - First login to get token
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "Admin@123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginApiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            loginContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var token = loginApiResponse!.Data!.Token;
        var userId = loginApiResponse.Data.UserId;

        // Add authorization header
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/AuthApi/users/{userId}/is-active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var isActive = JsonSerializer.Deserialize<bool>(content);
        Assert.True(isActive);
    }

    [Fact]
    public async Task IsUserActive_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.Parse("20000000-0000-0000-0000-000000000001");

        // Act
        var response = await _client.GetAsync($"/api/AuthApi/users/{userId}/is-active");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task IsUserActive_WithInactiveUser_ReturnsFalse()
    {
        // Arrange - Login first
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "Admin@123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginApiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            loginContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var token = loginApiResponse!.Data!.Token;
        var inactiveUserId = Guid.Parse("20000000-0000-0000-0000-000000000003");

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/AuthApi/users/{inactiveUserId}/is-active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var isActive = JsonSerializer.Deserialize<bool>(content);
        Assert.False(isActive);
    }

    [Fact]
    public async Task Login_MultipleConcurrentRequests_AllSucceed()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "Admin@123"
        };

        var tasks = new List<Task<HttpResponseMessage>>();

        // Act - Send 5 concurrent login requests
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(_client.PostAsJsonAsync("/api/AuthApi/login", loginRequest));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - All should succeed
        foreach (var response in responses)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}