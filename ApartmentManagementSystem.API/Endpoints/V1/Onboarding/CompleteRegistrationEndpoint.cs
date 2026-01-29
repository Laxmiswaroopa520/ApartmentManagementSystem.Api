using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class CompleteRegistrationEndpoint
    : Endpoint<CompleteRegistrationDto, ApiResponse<CompleteRegistrationResponseDto>>
{
    private readonly IOnboardingService _onboardingService;

    public CompleteRegistrationEndpoint(IOnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    public override void Configure()
    {
        Post("/v1/fast/onboarding/complete-registration");
        AllowAnonymous();

        Description(b => b
            .WithTags("Onboarding")
            .WithName("CompleteRegistration")
            .WithSummary("Complete resident registration")
            .WithDescription(@"
                Completes the registration process after OTP verification.
                Creates user credentials (username/password) and profile information.
                User status will be set to 'PendingFlatAllocation' after successful registration.
                Admin must assign a flat before the user can fully access the system.
            ")
            .Produces<ApiResponse<CompleteRegistrationResponseDto>>(200, "application/json")
            .Produces<ApiResponse<CompleteRegistrationResponseDto>>(400, "application/json")
        );
    }

    public override async Task HandleAsync(
        CompleteRegistrationDto req,
        CancellationToken ct)
    {
        try
        {
            // Call the service (unchanged from controller)
            var result = await _onboardingService.CompleteRegistrationAsync(req);

            // Return success response
            await SendAsync(
                ApiResponse<CompleteRegistrationResponseDto>
                    .SuccessResponse(result, "Registration completed successfully"),
                200,
                ct);
        }
        catch (Exception ex)
        {
            // Handle errors (username exists, OTP not verified, etc.)
            await SendAsync(
                ApiResponse<CompleteRegistrationResponseDto>
                    .ErrorResponse(ex.Message),
                400,
                ct);
        }
    }
}