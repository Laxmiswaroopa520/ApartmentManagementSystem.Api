using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ApartmentManagementSystem.API.Controllers.V1;
[ApiController]
[ApiVersion("1.0")] 
[Route("api/v{version:apiVersion}/[controller]")]
//[Route("api/[controller]")]
public class OnboardingApiController : ControllerBase
{
    private readonly IOnboardingService OnboardingService;
    private readonly IRoleRepository RoleRepository;
    public OnboardingApiController(
        IOnboardingService onboardingService,
        IRoleRepository roleRepository)
    {
        OnboardingService = onboardingService;
        RoleRepository = roleRepository;
    }

    // CREATE INVITE
    [HttpPost("create-invite")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> CreateInvite([FromBody] CreateUserInviteDto request)
    {
        // Validate ResidentType (1=Owner, 2=Tenant, 3=Staff)
        if (request.ResidentType < 1 || request.ResidentType > 3)
        {
            return BadRequest(ApiResponse<CreateInviteResponseDto>.ErrorResponse("Invalid resident type"));
        }
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await OnboardingService.CreateInviteAsync(request, userId);

            return Ok(ApiResponse<CreateInviteResponseDto>
                .SuccessResponse(result, "Invite created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CreateInviteResponseDto>
                .ErrorResponse(ex.Message));
        }
    }

    // VERIFY OTP
    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
    {
        try
        {
            var result = await OnboardingService.VerifyOtpAsync(request);
            return Ok(ApiResponse<VerifyOtpResponseDto>
                .SuccessResponse(result, "OTP verified successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<VerifyOtpResponseDto>
                .ErrorResponse(ex.Message));
        }
    }
    // COMPLETE REGISTRATION
    [HttpPost("complete-registration")]
    [AllowAnonymous]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationDto request)
    {
        try
        {
            var result = await OnboardingService.CompleteRegistrationAsync(request);
            return Ok(ApiResponse<CompleteRegistrationResponseDto>
                .SuccessResponse(result, "Registration completed successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CompleteRegistrationResponseDto>
                .ErrorResponse(ex.Message));
        }
    }
   // GET ROLES(USED BY WEB)
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await RoleRepository.GetAllAsync();

        return Ok(roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }));
    }
   
    [HttpGet("resident-types")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public IActionResult GetResidentTypes()
    {
        var residentTypes = Enum.GetValues(typeof(ResidentType))
            .Cast<ResidentType>()
            .Select(rt => new ResidentTypeDto
            {
                Id = (int)rt,
                Name = rt.ToString()
            })
            .ToList();

        return Ok(ApiResponse<List<ResidentTypeDto>>
            .SuccessResponse(residentTypes));
    }
}
