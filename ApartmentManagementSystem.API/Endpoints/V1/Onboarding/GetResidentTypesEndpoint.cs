/*using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Domain.Enums;
using FastEndpoints;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class GetResidentTypesEndpoint
    : EndpointWithoutRequest<ApiResponse<List<ResidentTypeDto>>>
{
    public override void Configure()
    {
        // CORRECTED: Simple route with version
        Get("v1/onboarding/fast/resident-types");

        Roles("SuperAdmin", "Manager");

        Description(b => b
            .WithTags("Onboarding") // Match your controller tag
            .WithName("GetResidentTypes")
            .WithSummary("Get all resident types")
            .WithDescription(@"
                Returns all available resident types in the system:
                - 1: Owner
                - 2: Tenant
                - 3: Staff
            ")
            .Produces<ApiResponse<List<ResidentTypeDto>>>(200, "application/json")
            .ProducesProblem(401)
            .ProducesProblem(403)
        );
    }

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
            ApiResponse<List<ResidentTypeDto>>.SuccessResponse(residentTypes),
            200,
            ct);
    }
}
*/

using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class GetResidentTypesEndpoint
    : EndpointWithoutRequest<ApiResponse<List<ResidentTypeDto>>>
{
    public override void Configure()
    {
        Get("onboardingApi/resident-types");
        Roles("SuperAdmin", "Manager");

        Description(b => b
            .WithTags("Onboarding")
            .WithName("GetResidentTypes")
            .WithSummary("Get all resident types")
            .WithDescription(@"
                Returns all available resident types in the system:
                - 1: Owner
                - 2: Tenant
                - 3: Staff
            ")
            .Produces<ApiResponse<List<ResidentTypeDto>>>(200, "application/json")
            .ProducesProblem(401)
            .ProducesProblem(403)
        );
    }

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
            ApiResponse<List<ResidentTypeDto>>.SuccessResponse(residentTypes),
            200,
            ct);
    }
}





