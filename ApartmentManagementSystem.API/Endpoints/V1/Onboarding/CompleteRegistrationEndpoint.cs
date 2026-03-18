using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using FastEndpoints;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

/// <summary>
/// Endpoint responsible for completing resident registration
/// after successful OTP verification.
/// 
/// This endpoint:
/// - Creates login credentials (username & password)
/// - Saves profile information
/// - Sets user status to PendingFlatAllocation
/// 
/// Anonymous access allowed.
/// </summary>
public class CompleteRegistrationEndpoint
    : Endpoint<CompleteRegistrationDto, ApiResponse<CompleteRegistrationResponseDto>>
{
    /// <summary>
    /// Service responsible for onboarding business logic.
    /// </summary>
    private readonly IOnboardingService OnboardingService;

    /// <summary>
    /// Constructor for injecting onboarding service.
    /// </summary>
    public CompleteRegistrationEndpoint(IOnboardingService onboardingService)
    {
        OnboardingService = onboardingService;
    }

    /// <summary>
    /// Configures endpoint route and Swagger metadata.
    /// </summary>
    public override void Configure()
    {
        Post("OnboardingApi/complete-registration");
        AllowAnonymous();

        Description(b => b
            .WithTags("OnboardingApi")
            .WithName("CompleteRegistration")
            .Produces<ApiResponse<CompleteRegistrationResponseDto>>(200, "application/json")
            .Produces<ApiResponse<CompleteRegistrationResponseDto>>(400, "application/json")
        );
    }

    /// <summary>
    /// Handles registration completion request.
    /// </summary>
    /// <param name="req">Registration completion request DTO.</param>
    /// <param name="ct">Cancellation token.</param>
    public override async Task HandleAsync(
        CompleteRegistrationDto req,
        CancellationToken ct)
    {
        try
        {
            var result = await OnboardingService
                .CompleteRegistrationAsync(req);

            await SendAsync(
                ApiResponse<CompleteRegistrationResponseDto>
                    .SuccessResponse(
                        result,
                        OnboardingMessages.RegistrationCompleted
                    ),
                200,
                ct);
        }
        catch (Exception ex)
        {
            await SendAsync(
                ApiResponse<CompleteRegistrationResponseDto>
                    .ErrorResponse(ex.Message),
                400,
                ct);
        }
    }
}

























