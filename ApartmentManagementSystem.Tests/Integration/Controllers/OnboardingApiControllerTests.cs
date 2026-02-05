using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApartmentManagementSystem.Tests.Integration.Controllers;

public class OnboardingApiControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public OnboardingApiControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    // Helper method to create authenticated client
    private async Task<HttpClient> CreateAuthenticatedClient()
    {
        var client = _factory.CreateClient();
        var token = await GetAdminTokenAsync(client);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    // Helper method to get admin token
    private async Task<string> GetAdminTokenAsync(HttpClient client)
    {
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "Admin@123"
        };

        var response = await client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return apiResponse!.Data!.Token;
    }

    #region CreateInvite Tests

   /*[Fact]
    public async Task CreateInvite_WithValidOwnerData_ReturnsCreatedInvite()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        var request = new CreateUserInviteDto
        {
           // FullName = "New Owner",
           FullName= "Test Admin",
            //PrimaryPhone = "9876543210",
            PrimaryPhone= "9999999999",
            //ResidentType = (int)ResidentType.Owner
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed: {response.StatusCode}, Body: {errorContent}");
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("New Owner", apiResponse.Data.FullName);
        Assert.Equal("9876543210", apiResponse.Data.PrimaryPhone);
        Assert.Equal("Owner", apiResponse.Data.ResidentType);
        Assert.NotEmpty(apiResponse.Data.OtpCode);
    }
 */
   /* [Fact]
    public async Task CreateInvite_WithValidTenantData_ReturnsCreatedInvite()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        var request = new CreateUserInviteDto
        {
            FullName = "New Tenant",
            PrimaryPhone = "9876543211",
            ResidentType = (int)ResidentType.Tenant
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Tenant", apiResponse.Data!.ResidentType);
    }

    [Fact]
    public async Task CreateInvite_WithValidStaffData_ReturnsCreatedInvite()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        var request = new CreateUserInviteDto
        {
            FullName = "Security Staff",
            PrimaryPhone = "9876543212",
            ResidentType = (int)ResidentType.Staff
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Staff", apiResponse.Data!.ResidentType);
    }
   */
  /*  [Fact]
    public async Task CreateInvite_WithInvalidResidentType_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        var request = new CreateUserInviteDto
        {
            FullName = "Test User",
            PrimaryPhone = "9876543213",
            ResidentType = 999 // Invalid
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Invalid resident type", apiResponse.Message);
    }
  */
/*
    [Fact]
    public async Task CreateInvite_WithExistingPhone_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        // First create an invite
        var request = new CreateUserInviteDto
        {
            FullName = "First User",
            PrimaryPhone = "9876543214",
            ResidentType = (int)ResidentType.Owner
        };
        await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Try to create another with same phone
        var duplicateRequest = new CreateUserInviteDto
        {
            FullName = "Second User",
            PrimaryPhone = "9876543214",
            ResidentType = (int)ResidentType.Tenant
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", duplicateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    */
    [Fact]
    public async Task CreateInvite_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient(); // Create client without auth

        var request = new CreateUserInviteDto
        {
            FullName = "Test User",
            PrimaryPhone = "9876543215",
            ResidentType = (int)ResidentType.Owner
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region VerifyOtp Tests

   /* [Fact]
    public async Task VerifyOtp_WithValidOtp_ReturnsSuccess()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        // Create invite first
        var createRequest = new CreateUserInviteDto
        {
            FullName = "OTP Test User",
            PrimaryPhone = "9876543216",
            ResidentType = (int)ResidentType.Owner
        };

        var createResponse = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createApiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var otpCode = createApiResponse!.Data!.OtpCode;

        // Act - Verify OTP
        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "9876543216",
            OtpCode = otpCode
        };

        var verifyResponse = await client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

        var verifyContent = await verifyResponse.Content.ReadAsStringAsync();
        var verifyApiResponse = JsonSerializer.Deserialize<ApiResponse<VerifyOtpResponseDto>>(
            verifyContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(verifyApiResponse);
        Assert.True(verifyApiResponse.Success);
        Assert.True(verifyApiResponse.Data!.Success);
    }*/

   /* [Fact]
    public async Task VerifyOtp_WithInvalidOtp_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        // Create invite first
        var createRequest = new CreateUserInviteDto
        {
            FullName = "Invalid OTP User",
            PrimaryPhone = "9876543217",
            ResidentType = (int)ResidentType.Owner
        };

        await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);

        // Act - Try with wrong OTP
        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "9876543217",
            OtpCode = "999999"
        };

        var response = await client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }*/

    [Fact]
    public async Task VerifyOtp_WithNonExistentPhone_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient(); // Fresh client

        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "0000000000",
            OtpCode = "123456"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region CompleteRegistration Tests

   /* [Fact]
    public async Task CompleteRegistration_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        // Step 1: Create invite
        var createRequest = new CreateUserInviteDto
        {
            FullName = "Registration User",
            PrimaryPhone = "9876543218",
            ResidentType = (int)ResidentType.Owner
        };

        var createResponse = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createApiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var otpCode = createApiResponse!.Data!.OtpCode;

        // Step 2: Verify OTP
        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "9876543218",
            OtpCode = otpCode
        };
        await client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Step 3: Complete Registration
        var completeRequest = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543218",
            FullName = "Registration User Updated",
            SecondaryPhone = "9876543219",
            Email = "reguser@test.com",
            Username = "reguser",
            Password = "Password@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", completeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CompleteRegistrationResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("reguser", apiResponse.Data!.Username);
        Assert.Equal("PendingFlatAllocation", apiResponse.Data.Status);
    }*/
/*
    [Fact]
    public async Task CompleteRegistration_WithoutOtpVerification_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        // Create invite but don't verify OTP
        var createRequest = new CreateUserInviteDto
        {
            FullName = "No OTP User",
            PrimaryPhone = "9876543220",
            ResidentType = (int)ResidentType.Owner
        };
        await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);

        // Try to complete registration without OTP verification
        var completeRequest = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543220",
            FullName = "No OTP User",
            Username = "nooptuser",
            Password = "Password@123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", completeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }*/
/*
    [Fact]
    public async Task CompleteRegistration_WithDuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        // Complete first registration
        var createRequest1 = new CreateUserInviteDto
        {
            FullName = "First User",
            PrimaryPhone = "9876543221",
            ResidentType = (int)ResidentType.Owner
        };
        var createResponse1 = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest1);
        var createContent1 = await createResponse1.Content.ReadAsStringAsync();
        var createApiResponse1 = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent1,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        var otpCode1 = createApiResponse1!.Data!.OtpCode;

        await client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", new VerifyOtpDto
        {
            PrimaryPhone = "9876543221",
            OtpCode = otpCode1
        });

        await client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543221",
            FullName = "First User",
            Username = "duplicateuser",
            Password = "Password@123"
        });

        // Try second registration with same username
        var createRequest2 = new CreateUserInviteDto
        {
            FullName = "Second User",
            PrimaryPhone = "9876543222",
            ResidentType = (int)ResidentType.Tenant
        };
        var createResponse2 = await client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest2);
        var createContent2 = await createResponse2.Content.ReadAsStringAsync();
        var createApiResponse2 = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent2,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        var otpCode2 = createApiResponse2!.Data!.OtpCode;

        await client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", new VerifyOtpDto
        {
            PrimaryPhone = "9876543222",
            OtpCode = otpCode2
        });

        // Act
        var response = await client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543222",
            FullName = "Second User",
            Username = "duplicateuser", // Same username
            Password = "Password@123"
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
*/
    #endregion

    #region GetRoles Tests

    [Fact]
    public async Task GetRoles_ReturnsAllRoles()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/OnboardingApi/roles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var roles = JsonSerializer.Deserialize<List<RoleDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(roles);
        Assert.NotEmpty(roles);
        Assert.Contains(roles, r => r.Name == "SuperAdmin");
        Assert.Contains(roles, r => r.Name == "ResidentOwner");
    }

    #endregion

    #region GetResidentTypes Tests


    [Fact]
    public async Task GetResidentTypes_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient(); // Fresh client without auth

        // Act
        var response = await client.GetAsync("/api/OnboardingApi/resident-types");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion
}






/*
    [Fact]
    public async Task GetResidentTypes_WithAuthorization_ReturnsResidentTypes()
    {
        // Arrange
        var client = await CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/OnboardingApi/resident-types");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ResidentTypeDto>>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(3, apiResponse.Data.Count);
        Assert.Contains(apiResponse.Data, rt => rt.Name == "Owner");
        Assert.Contains(apiResponse.Data, rt => rt.Name == "Tenant");
        Assert.Contains(apiResponse.Data, rt => rt.Name == "Staff");
    }

    [Fact]
    public async Task GetResidentTypes_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient(); // Fresh client without auth

        // Act
        var response = await client.GetAsync("/api/OnboardingApi/resident-types");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion
}
*/
































/*
public class OnboardingApiControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OnboardingApiControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var loginRequest = new LoginRequestDto
        {
            Username = "testadmin",
            Password = "Admin@123"
        };

        var response = await _client.PostAsJsonAsync("/api/AuthApi/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return apiResponse!.Data!.Token;
    }

    #region CreateInvite Tests

    [Fact]
    public async Task CreateInvite_WithValidOwnerData_ReturnsCreatedInvite()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateUserInviteDto
        {
            FullName = "New Owner",
            PrimaryPhone = "9876543210",
            ResidentType = (int)ResidentType.Owner
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("New Owner", apiResponse.Data.FullName);
        Assert.Equal("9876543210", apiResponse.Data.PrimaryPhone);
        Assert.Equal("Owner", apiResponse.Data.ResidentType);
        Assert.NotEmpty(apiResponse.Data.OtpCode);
    }

    [Fact]
    public async Task CreateInvite_WithValidTenantData_ReturnsCreatedInvite()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateUserInviteDto
        {
            FullName = "New Tenant",
            PrimaryPhone = "9876543211",
            ResidentType = (int)ResidentType.Tenant
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Tenant", apiResponse.Data!.ResidentType);
    }

    [Fact]
    public async Task CreateInvite_WithValidStaffData_ReturnsCreatedInvite()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateUserInviteDto
        {
            FullName = "Security Staff",
            PrimaryPhone = "9876543212",
            ResidentType = (int)ResidentType.Staff
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("Staff", apiResponse.Data!.ResidentType);
    }

    [Fact]
    public async Task CreateInvite_WithInvalidResidentType_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateUserInviteDto
        {
            FullName = "Test User",
            PrimaryPhone = "9876543213",
            ResidentType = 999 // Invalid
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Contains("Invalid resident type", apiResponse.Message);
    }

    [Fact]
    public async Task CreateInvite_WithExistingPhone_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // First create an invite
        var request = new CreateUserInviteDto
        {
            FullName = "First User",
            PrimaryPhone = "9876543214",
            ResidentType = (int)ResidentType.Owner
        };
        await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Try to create another with same phone
        var duplicateRequest = new CreateUserInviteDto
        {
            FullName = "Second User",
            PrimaryPhone = "9876543214",
            ResidentType = (int)ResidentType.Tenant
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", duplicateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateInvite_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        var request = new CreateUserInviteDto
        {
            FullName = "Test User",
            PrimaryPhone = "9876543215",
            ResidentType = (int)ResidentType.Owner
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region VerifyOtp Tests

    [Fact]
    public async Task VerifyOtp_WithValidOtp_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create invite first
        var createRequest = new CreateUserInviteDto
        {
            FullName = "OTP Test User",
            PrimaryPhone = "9876543216",
            ResidentType = (int)ResidentType.Owner
        };

        var createResponse = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createApiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var otpCode = createApiResponse!.Data!.OtpCode;

        // Act - Verify OTP
        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "9876543216",
            OtpCode = otpCode
        };

        var verifyResponse = await _client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

        var verifyContent = await verifyResponse.Content.ReadAsStringAsync();
        var verifyApiResponse = JsonSerializer.Deserialize<ApiResponse<VerifyOtpResponseDto>>(
            verifyContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(verifyApiResponse);
        Assert.True(verifyApiResponse.Success);
        Assert.True(verifyApiResponse.Data!.Success);
    }

    [Fact]
    public async Task VerifyOtp_WithInvalidOtp_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create invite first
        var createRequest = new CreateUserInviteDto
        {
            FullName = "Invalid OTP User",
            PrimaryPhone = "9876543217",
            ResidentType = (int)ResidentType.Owner
        };

        await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);

        // Act - Try with wrong OTP
        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "9876543217",
            OtpCode = "999999"
        };

        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VerifyOtp_WithNonExistentPhone_ReturnsBadRequest()
    {
        // Arrange
        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "0000000000",
            OtpCode = "123456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region CompleteRegistration Tests

    [Fact]
    public async Task CompleteRegistration_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Step 1: Create invite
        var createRequest = new CreateUserInviteDto
        {
            FullName = "Registration User",
            PrimaryPhone = "9876543218",
            ResidentType = (int)ResidentType.Owner
        };

        var createResponse = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createApiResponse = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var otpCode = createApiResponse!.Data!.OtpCode;

        // Step 2: Verify OTP
        var verifyRequest = new VerifyOtpDto
        {
            PrimaryPhone = "9876543218",
            OtpCode = otpCode
        };
        await _client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", verifyRequest);

        // Step 3: Complete Registration
        var completeRequest = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543218",
            FullName = "Registration User Updated",
            SecondaryPhone = "9876543219",
            Email = "reguser@test.com",
            Username = "reguser",
            Password = "Password@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", completeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<CompleteRegistrationResponseDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal("reguser", apiResponse.Data!.Username);
        Assert.Equal("PendingFlatAllocation", apiResponse.Data.Status);
    }

    [Fact]
    public async Task CompleteRegistration_WithoutOtpVerification_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create invite but don't verify OTP
        var createRequest = new CreateUserInviteDto
        {
            FullName = "No OTP User",
            PrimaryPhone = "9876543220",
            ResidentType = (int)ResidentType.Owner
        };
        await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest);

        // Try to complete registration without OTP verification
        var completeRequest = new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543220",
            FullName = "No OTP User",
            Username = "nooptuser",
            Password = "Password@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", completeRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CompleteRegistration_WithDuplicateUsername_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Complete first registration
        var createRequest1 = new CreateUserInviteDto
        {
            FullName = "First User",
            PrimaryPhone = "9876543221",
            ResidentType = (int)ResidentType.Owner
        };
        var createResponse1 = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest1);
        var createContent1 = await createResponse1.Content.ReadAsStringAsync();
        var createApiResponse1 = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent1,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        var otpCode1 = createApiResponse1!.Data!.OtpCode;

        await _client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", new VerifyOtpDto
        {
            PrimaryPhone = "9876543221",
            OtpCode = otpCode1
        });

        await _client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543221",
            FullName = "First User",
            Username = "duplicateuser",
            Password = "Password@123"
        });

        // Try second registration with same username
        var createRequest2 = new CreateUserInviteDto
        {
            FullName = "Second User",
            PrimaryPhone = "9876543222",
            ResidentType = (int)ResidentType.Tenant
        };
        var createResponse2 = await _client.PostAsJsonAsync("/api/OnboardingApi/create-invite", createRequest2);
        var createContent2 = await createResponse2.Content.ReadAsStringAsync();
        var createApiResponse2 = JsonSerializer.Deserialize<ApiResponse<CreateInviteResponseDto>>(
            createContent2,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        var otpCode2 = createApiResponse2!.Data!.OtpCode;

        await _client.PostAsJsonAsync("/api/OnboardingApi/verify-otp", new VerifyOtpDto
        {
            PrimaryPhone = "9876543222",
            OtpCode = otpCode2
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/OnboardingApi/complete-registration", new CompleteRegistrationDto
        {
            PrimaryPhone = "9876543222",
            FullName = "Second User",
            Username = "duplicateuser", // Same username
            Password = "Password@123"
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region GetRoles Tests

    [Fact]
    public async Task GetRoles_ReturnsAllRoles()
    {
        // Act
        var response = await _client.GetAsync("/api/OnboardingApi/roles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var roles = JsonSerializer.Deserialize<List<RoleDto>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(roles);
        Assert.NotEmpty(roles);
        Assert.Contains(roles, r => r.Name == "SuperAdmin");
        Assert.Contains(roles, r => r.Name == "ResidentOwner");
    }

    #endregion

    #region GetResidentTypes Tests

    [Fact]
    public async Task GetResidentTypes_WithAuthorization_ReturnsResidentTypes()
    {
        // Arrange
        var token = await GetAdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/OnboardingApi/resident-types");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ResidentTypeDto>>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal(3, apiResponse.Data.Count);
        Assert.Contains(apiResponse.Data, rt => rt.Name == "Owner");
        Assert.Contains(apiResponse.Data, rt => rt.Name == "Tenant");
        Assert.Contains(apiResponse.Data, rt => rt.Name == "Staff");
    }

    [Fact]
    public async Task GetResidentTypes_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/OnboardingApi/resident-types");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion
}


*/