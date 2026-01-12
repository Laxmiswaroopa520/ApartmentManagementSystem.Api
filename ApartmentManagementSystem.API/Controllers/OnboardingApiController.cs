using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OnboardingApiController : ControllerBase
{
    private readonly IOnboardingService _onboardingService;
    private readonly IRoleRepository _roleRepository;

    public OnboardingApiController(
        IOnboardingService onboardingService,
        IRoleRepository roleRepository)
    {
        _onboardingService = onboardingService;
        _roleRepository = roleRepository;
    }

    // -------------------------
    // CREATE INVITE
    // -------------------------
    [HttpPost("create-invite")]
    [Authorize(Roles = "SuperAdmin,President,Secretary")]
    public async Task<IActionResult> CreateInvite([FromBody] CreateUserInviteDto request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _onboardingService.CreateInviteAsync(request, userId);

            return Ok(ApiResponse<CreateInviteResponseDto>
                .SuccessResponse(result, "Invite created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CreateInviteResponseDto>
                .ErrorResponse(ex.Message));
        }
    }

    // -------------------------
    // VERIFY OTP
    // -------------------------
    [HttpPost("verify-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto request)
    {
        try
        {
            var result = await _onboardingService.VerifyOtpAsync(request);
            return Ok(ApiResponse<VerifyOtpResponseDto>
                .SuccessResponse(result, "OTP verified successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<VerifyOtpResponseDto>
                .ErrorResponse(ex.Message));
        }
    }

    // -------------------------
    // COMPLETE REGISTRATION
    // -------------------------
    [HttpPost("complete-registration")]
    [AllowAnonymous]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationDto request)
    {
        try
        {
            var result = await _onboardingService.CompleteRegistrationAsync(request);
            return Ok(ApiResponse<CompleteRegistrationResponseDto>
                .SuccessResponse(result, "Registration completed successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CompleteRegistrationResponseDto>
                .ErrorResponse(ex.Message));
        }
    }

    // -------------------------
    // GET ROLES (USED BY WEB)
    // -------------------------
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleRepository.GetAllAsync();

        return Ok(roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name
        }));
    }
}
