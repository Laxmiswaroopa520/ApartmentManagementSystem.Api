using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

/// <summary>
/// Endpoint responsible for retrieving all available resident types.
/// 
/// Allowed roles:
/// - SuperAdmin
/// - Manager
/// </summary>
public class GetResidentTypesEndpoint
    : EndpointWithoutRequest<ApiResponse<List<ResidentTypeDto>>>
{
    /// <summary>
    /// Configures route, roles, and Swagger documentation.
    /// </summary>
    public override void Configure()
    {
        Get("OnboardingApi/resident-types");
        Roles("SuperAdmin", "Manager");

        Description(b => b
            .WithTags("OnboardingApi")
            .WithName("GetResidentTypes")
            .WithDescription(ResidentTypeMessages.ResidentTypesDescription)             //considered description in constants file
            .Produces<ApiResponse<List<ResidentTypeDto>>>(200, "application/json")
            .ProducesProblem(401)
            .ProducesProblem(403)
        );
    }

    /// <summary>
    /// Handles request to fetch all resident types.
    /// </summary>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var residentTypes = Enum.GetValues(typeof(ResidentType))
            .Cast<ResidentType>()
            .Select(rt => new ResidentTypeDto
            {
                Id = (int)rt,
                Name = rt.ToString()
            })
            .ToList();

        await SendAsync(
            ApiResponse<List<ResidentTypeDto>>
                .SuccessResponse(residentTypes),
            200,
            ct);
    }
}



































