using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Domain.Constants;
using FastEndpoints;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

/// <summary>
/// Endpoint responsible for retrieving all system roles.
/// Anonymous access allowed.
/// </summary>
public class GetRolesEndpoint : EndpointWithoutRequest<List<RoleDto>>
{
    private readonly IUnitOfWork UoW;

    public GetRolesEndpoint(IUnitOfWork unitOfWork)
    {
        UoW = unitOfWork;
    }

    public override void Configure()
    {
        Get("OnboardingApi/roles");
        AllowAnonymous();
        Description(b => b
            .WithTags("OnboardingApi")
            .WithName("GetRoles")
            .WithSummary("Get all available system roles")
            .WithDescription(RoleMessages.GetRolesDescription)
            .Produces<List<RoleDto>>(200, "application/json")
        );
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await UoW.Roles.GetAllAsync();

        var roleDtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        await SendOkAsync(roleDtos, ct);
    }
}














/*
using FastEndpoints;
using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.API.Endpoints.V1.Onboarding;

/// <summary>
/// Endpoint responsible for retrieving all system roles.
/// Anonymous access allowed.
/// </summary>
public class GetRolesEndpoint : EndpointWithoutRequest<List<RoleDto>>
{
    /// <summary>
    /// Repository used to fetch roles from database.
    /// </summary>
    private readonly IRoleRepository RoleRepository;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    public GetRolesEndpoint(IRoleRepository roleRepository)
    {
        RoleRepository = roleRepository;
    }

    /// <summary>
    /// Configures route and Swagger metadata.
    /// </summary>
    public override void Configure()
    {
        Get("OnboardingApi/roles");
        AllowAnonymous();

        Description(b => b
            .WithTags("OnboardingApi")
            .WithName("GetRoles")
            .WithSummary("Get all available system roles")
            .WithDescription(RoleMessages.GetRolesDescription)
            .Produces<List<RoleDto>>(200, "application/json")
        );
    }

    /// <summary>
    /// Handles request to retrieve all roles.
    /// </summary>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await RoleRepository.GetAllAsync();

        var roleDtos = roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }).ToList();

        await SendOkAsync(roleDtos, ct);
    }
}


*/













































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
