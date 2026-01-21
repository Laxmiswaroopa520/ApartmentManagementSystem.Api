using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Manager")]
public class CommunityMembersApiController : ControllerBase
{
    private readonly ICommunityMemberService _communityService;

    public CommunityMembersApiController(ICommunityMemberService communityService)
    {
        _communityService = communityService;
    }

    /// <summary>
    /// Get all community members (President, Secretary, Treasurer)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllCommunityMembers()
    {
        try
        {
            var members = await _communityService.GetAllCommunityMembersAsync();
            return Ok(ApiResponse<List<CommunityMemberDto>>.SuccessResponse(
                members,
                "Community members retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<CommunityMemberDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get residents eligible for community roles (owners with flats assigned)
    /// </summary>
    [HttpGet("eligible-residents")]
    public async Task<IActionResult> GetEligibleResidents()
    {
        try
        {
            var residents = await _communityService.GetEligibleResidentsForCommunityRoleAsync();
            return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                residents,
                "Eligible residents retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ResidentListDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Assign community role to a resident
    /// </summary>
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignCommunityRole([FromBody] AssignCommunityRoleDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _communityService.AssignCommunityRoleAsync(dto, userId);

            return Ok(ApiResponse<CommunityMemberDto>.SuccessResponse(
                result,
                $"{dto.CommunityRole} role assigned successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityMemberDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Remove community role from a member
    /// </summary>
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveCommunityRole([FromBody] RemoveCommunityRoleDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _communityService.RemoveCommunityRoleAsync(dto, userId);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Community role removed successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get specific community member details
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCommunityMember(Guid userId)
    {
        try
        {
            var member = await _communityService.GetCommunityMemberByUserIdAsync(userId);

            if (member == null)
            {
                return NotFound(ApiResponse<CommunityMemberDto>.ErrorResponse("Community member not found"));
            }

            return Ok(ApiResponse<CommunityMemberDto>.SuccessResponse(
                member,
                "Community member retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityMemberDto>.ErrorResponse(ex.Message));
        }
    }
}