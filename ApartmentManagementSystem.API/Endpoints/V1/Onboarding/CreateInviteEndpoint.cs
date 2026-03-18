using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

/// <summary>
/// Endpoint responsible for creating invitations for new residents.
/// 
/// Allowed roles:
/// - SuperAdmin
/// - Manager
/// 
/// This endpoint:
/// - Validates resident type
/// - Generates OTP
/// - Sends invitation to provided phone number
/// </summary>
public class CreateInviteEndpoint
    : Endpoint<CreateUserInviteDto, ApiResponse<CreateInviteResponseDto>>
{
    /// <summary>
    /// Service responsible for onboarding business logic.
    /// </summary>
    private readonly IOnboardingService OnboardingService;

    /// <summary>
    /// Constructor for injecting onboarding service.
    /// </summary>
    public CreateInviteEndpoint(IOnboardingService onboardingService)
    {
        OnboardingService = onboardingService;
    }

    /// <summary>
    /// Configures endpoint route, roles, and Swagger documentation.
    /// </summary>
    public override void Configure()
    {
        Post("OnboardingApi/create-invite");
        Roles("SuperAdmin", "Manager");

        Description(b => b
            .WithTags("OnboardingApi")
            .WithName("CreateInvite")
            .Produces<ApiResponse<CreateInviteResponseDto>>(200, "application/json")
            .Produces<ApiResponse<CreateInviteResponseDto>>(400, "application/json")
            .ProducesProblem(401)
            .ProducesProblem(403)
        );
    }

    /// <summary>
    /// Handles invite creation request.
    /// </summary>
    /// <param name="req">Invite request DTO.</param>
    /// <param name="ct">Cancellation token.</param>
    public override async Task HandleAsync(
        CreateUserInviteDto req,
        CancellationToken ct)
    {
        if (req.ResidentType < 1 || req.ResidentType > 3)
        {
            await SendAsync(
                ApiResponse<CreateInviteResponseDto>
                    .ErrorResponse(InviteMessages.InvalidResidentType),
                400,
                ct);
            return;
        }

        try
        {
            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await OnboardingService
                .CreateInviteAsync(req, userId);

            await SendAsync(
                ApiResponse<CreateInviteResponseDto>
                    .SuccessResponse(
                        result,
                        InviteMessages.InviteCreatedSuccessfully),
                200,
                ct);
        }
        catch (Exception ex)
        {
            await SendAsync(
                ApiResponse<CreateInviteResponseDto>
                    .ErrorResponse(ex.Message),
                400,
                ct);
        }
    }
}


































