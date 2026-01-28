using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1;

[ApiController]             //automatic model validation no need to check again and again for model validation..automatically returns 400 bad request..
[Route("api/[controller]")]     //controller name without controller ..
[Authorize(Roles = "SuperAdmin,Manager")]
public class CommunityMembersApiController : ControllerBase
{
    private readonly ICommunityMemberService CommunityService;

    public CommunityMembersApiController(ICommunityMemberService communityService)
    {
        CommunityService = communityService;
    }
    /// Get all community members (President, Secretary, Treasurer)
    [HttpGet]
    public async Task<IActionResult> GetAllCommunityMembers()
    {
        try
        {
            var members = await CommunityService.GetAllCommunityMembersAsync();
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

    /// Get residents eligible for community roles (owners with flats assigned)
    [HttpGet("eligible-residents")]
    public async Task<IActionResult> GetEligibleResidents()
    {
        try
        {
            var residents = await CommunityService.GetEligibleResidentsForCommunityRoleAsync();
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
    /// Assign community role to a resident
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignCommunityRole([FromBody] AssignCommunityRoleDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await CommunityService.AssignCommunityRoleAsync(dto, userId);

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

    /// Remove community role from a member
    [HttpPost("remove-role")]
    public async Task<IActionResult> RemoveCommunityRole([FromBody] RemoveCommunityRoleDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await CommunityService.RemoveCommunityRoleAsync(dto, userId);

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
    /// Get specific community member details
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCommunityMember(Guid userId)
    {
        try
        {
            var member = await CommunityService.GetCommunityMemberByUserIdAsync(userId);

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