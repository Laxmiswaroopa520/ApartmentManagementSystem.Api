using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class VerifyOtpEndpoint
    : Endpoint<VerifyOtpDto, ApiResponse<VerifyOtpResponseDto>>
{
    private readonly IOnboardingService OnboardingService;

    public VerifyOtpEndpoint(IOnboardingService onboardingService)
    {
        OnboardingService = onboardingService;
    }

    public override void Configure()
    {
        Post("OnboardingApi/verify-otp");
        AllowAnonymous();

        Description(b => b
            .WithTags("OnboardingApi")
            .WithName("VerifyOtp")
            .WithSummary("Verify OTP sent to resident's phone")
            .WithDescription(@"
                Verifies the OTP code sent to the resident's phone number.
                OTP is valid for 10 minutes from generation.
                After successful verification, user can proceed to complete registration.
            ")
            .Produces<ApiResponse<VerifyOtpResponseDto>>(200, "application/json")
            .Produces<ApiResponse<VerifyOtpResponseDto>>(400, "application/json")
        );
    }

    public override async Task HandleAsync(VerifyOtpDto req, CancellationToken ct)
    {
        try
        {
            var result = await OnboardingService.VerifyOtpAsync(req);

            await SendAsync(
                ApiResponse<VerifyOtpResponseDto>
                    .SuccessResponse(result, "OTP verified successfully"),
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
