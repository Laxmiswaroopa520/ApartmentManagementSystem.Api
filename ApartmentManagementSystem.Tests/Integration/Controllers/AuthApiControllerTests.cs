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
    private readonly CustomWebApplicationFactory Factory;

    public AuthApiControllerTests(CustomWebApplicationFactory factory)
    {
        Factory = factory;
    }

    // Helper to create fresh client
    private HttpClient CreateClient() => Factory.CreateClient();
    //tests::http->controller->service->database->response
    [Fact]
    public async Task Login_WithInvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateClient();                //creates real http client
        var loginRequest = new LoginRequestDto
        {
            Username = "nonexistentuser",
            Password = "SomePassword@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Invalid credentials", apiResponse.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "WrongPassword@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Invalid credentials", apiResponse.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithEmptyUsername_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var loginRequest = new LoginRequestDto
        {
            Username = "",
            Password = "Password@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        // Try to deserialize - if it fails, just check the response contains "required"
        try
        {
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(apiResponse);
            Assert.False(apiResponse.Success);
            Assert.Contains("required", apiResponse.Message, StringComparison.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            // If deserialization fails, just verify the content mentions validation
            Assert.Contains("required", content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = ""
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(apiResponse);
            Assert.False(apiResponse.Success);
            Assert.Contains("required", apiResponse.Message, StringComparison.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            Assert.Contains("required", content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task Login_WithWhitespaceCredentials_ReturnsBadRequest()
    {
        // Arrange
        var client = CreateClient();
        var loginRequest = new LoginRequestDto
        {
            Username = "   ",
            Password = "   "
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

  

    [Fact]
    public async Task IsUserActive_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateClient();
        var userId = Guid.Parse("20000000-0000-0000-0000-000000000001");

        // Act
        var response = await client.GetAsync($"/api/AuthApi/users/{userId}/is-active");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task IsUserActive_WithInactiveUser_ReturnsFalse()
    {
        // Arrange - Login first with fresh client
        var client = CreateClient();

        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "Admin@123"
        };

        var loginResponse = await client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginApiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            loginContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var token = loginApiResponse!.Data!.Token;
        var inactiveUserId = Guid.Parse("20000000-0000-0000-0000-000000000003");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"/api/AuthApi/users/{inactiveUserId}/is-active");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var isActive = JsonSerializer.Deserialize<bool>(content);
        Assert.False(isActive);
    }

  
    
}