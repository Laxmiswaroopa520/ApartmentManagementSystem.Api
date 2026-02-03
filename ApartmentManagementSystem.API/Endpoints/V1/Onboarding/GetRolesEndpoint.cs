/*using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.Interfaces.Repositories;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class GetRolesEndpoint : EndpointWithoutRequest<List<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesEndpoint(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public override void Configure()
    {
        // CORRECTED: Simple route, "api" prefix is added automatically
        Get("v1/onboarding/fast/roles");

        AllowAnonymous();

        Description(b => b
            .WithTags("Onboarding") // Match your controller tag
            .WithName("GetRoles")
            .WithSummary("Get all available system roles")
            .WithDescription("Returns a list of all roles available in the system. Used by web interface for role selection.")
            .Produces<List<RoleDto>>(200, "application/json")
        );
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await _roleRepository.GetAllAsync();

        var roleDtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        await SendOkAsync(roleDtos, ct);
    }
}


*/
using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.Interfaces.Repositories;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class GetRolesEndpoint : EndpointWithoutRequest<List<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesEndpoint(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public override void Configure()
    {
        // Route WITHOUT leading slash (RoutePrefix "api" is added automatically)
        Get("onboardingApi/roles");
        AllowAnonymous();
        
        Description(b => b
            .WithTags("Onboarding") // This creates the tag grouping in Swagger
            .WithName("GetRoles")
            .WithSummary("Get all available system roles")
            .WithDescription("Returns a list of all roles available in the system. Used by web interface for role selection.")
            .Produces<List<RoleDto>>(200, "application/json")
        );
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await _roleRepository.GetAllAsync();
        
        var roleDtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        await SendOkAsync(roleDtos, ct);
    }
}









/*using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using NSwag.Annotations;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

public class GetRolesEndpoint
    : EndpointWithoutRequest<List<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesEndpoint(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    //https://localhost:7093/api/v1/fast/onboarding/roles


    public override void Configure()
    {
        // Get("v{version}/fast/onboarding/roles");
        Get("onboarding/roles");
        AllowAnonymous(); // Change to authenticated if needed
        //Version(1);
        Description(b => b
            .WithTags("Onboarding")
            .WithName("GetRoles")
            .WithSummary("Get all available system roles")
            .WithDescription("Returns a list of all roles available in the system. Used by web interface for role selection.")
            .Produces<List<RoleDto>>(200, "application/json")
        );
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Get all roles from database
        var roles = await _roleRepository.GetAllAsync();

        // Map to DTOs
        var roleDtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        // Return the list
        await SendAsync(roleDtos, 200, ct);
    }
}
*/
