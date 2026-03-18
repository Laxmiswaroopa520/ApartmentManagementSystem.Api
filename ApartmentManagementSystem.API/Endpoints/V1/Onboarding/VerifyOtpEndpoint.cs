using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

/// <summary>
/// Endpoint responsible for verifying OTP sent to resident.
/// Anonymous access allowed.
/// </summary>
public class VerifyOtpEndpoint
    : Endpoint<VerifyOtpDto, ApiResponse<VerifyOtpResponseDto>>
{
    /// <summary>
    /// Service responsible for onboarding logic.
    /// </summary>
    private readonly IOnboardingService OnboardingService;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    public VerifyOtpEndpoint(IOnboardingService onboardingService)
    {
        OnboardingService = onboardingService;
    }

    /// <summary>
    /// Configures route and Swagger metadata.
    /// </summary>
    public override void Configure()
    {
        Post("OnboardingApi/verify-otp");
        AllowAnonymous();

        Description(b => b
            .WithTags("OnboardingApi")
            .WithName("VerifyOtp")
            .WithDescription(OtpMessages.VerifyOtpDescription)
            .Produces<ApiResponse<VerifyOtpResponseDto>>(200, "application/json")
            .Produces<ApiResponse<VerifyOtpResponseDto>>(400, "application/json")
        );
    }

    /// <summary>
    /// Handles OTP verification request.
    /// </summary>
    public override async Task HandleAsync(
        VerifyOtpDto req,
        CancellationToken ct)
    {
        try
        {
            var result = await OnboardingService
                .VerifyOtpAsync(req);

            await SendAsync(
                ApiResponse<VerifyOtpResponseDto>
                    .SuccessResponse(
                        result,
                        OtpMessages.OtpVerifiedSuccessfully),
                200,
                ct);
        }
        catch (Exception ex)
        {
            await SendAsync(
                ApiResponse<VerifyOtpResponseDto>
                    .ErrorResponse(ex.Message),
                400,
                ct);
        }
    }
}









































