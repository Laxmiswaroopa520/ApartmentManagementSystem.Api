using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class CreateInviteEndpoint
    : Endpoint<CreateUserInviteDto, ApiResponse<CreateInviteResponseDto>>
{
    private readonly IOnboardingService _onboardingService;

    public CreateInviteEndpoint(IOnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    public override void Configure()
    {
        Post("/v1/fast/onboarding/create-invite");
        Roles("SuperAdmin", "Manager");

        Description(b => b
            .WithTags("Onboarding")
            .WithName("CreateInvite")
            .WithSummary("Create invitation for new resident")
            .WithDescription(@"
                Creates an invitation for a new resident (Owner/Tenant/Staff).
                Generates OTP and sends to the provided phone number.
                
                ResidentType values:
                - 1: Owner
                - 2: Tenant
                - 3: Staff
            ")
            .Produces<ApiResponse<CreateInviteResponseDto>>(200, "application/json")
            .Produces<ApiResponse<CreateInviteResponseDto>>(400, "application/json")
            .ProducesProblem(401)
            .ProducesProblem(403)
        );
    }

    public override async Task HandleAsync(
        CreateUserInviteDto req,
        CancellationToken ct)
    {
        // Validate ResidentType
        if (req.ResidentType < 1 || req.ResidentType > 3)
        {
            await SendAsync(
                ApiResponse<CreateInviteResponseDto>
                    .ErrorResponse("Invalid resident type. Must be 1 (Owner), 2 (Tenant), or 3 (Staff)"),
                400,
                ct);
            return;
        }

        try
        {
            // Extract current user's ID from JWT claims
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Call the service (unchanged from controller)
            var result = await _onboardingService.CreateInviteAsync(req, userId);

            // Return success response
            await SendAsync(
                ApiResponse<CreateInviteResponseDto>
                    .SuccessResponse(result, "Invite created successfully"),
                200,
                ct);
        }
        catch (Exception ex)
        {
            // Handle business logic errors
            await SendAsync(
                ApiResponse<CreateInviteResponseDto>
                    .ErrorResponse(ex.Message),
                400,
                ct);
        }
    }
}