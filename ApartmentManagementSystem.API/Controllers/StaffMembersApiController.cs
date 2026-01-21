using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
public class StaffMembersApiController : ControllerBase
{
    private readonly IStaffMemberService _staffService;

    public StaffMembersApiController(IStaffMemberService staffService)
    {
        _staffService = staffService;
    }

    /// <summary>
    /// Get all staff members
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllStaffMembers()
    {
        try
        {
            var staff = await _staffService.GetAllStaffMembersAsync();
            return Ok(ApiResponse<List<StaffMemberDto>>.SuccessResponse(
                staff,
                "Staff members retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<StaffMemberDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get staff members by type (Plumber, Security, etc.)
    /// </summary>
    [HttpGet("by-type/{staffType}")]
    public async Task<IActionResult> GetStaffMembersByType(string staffType)
    {
        try
        {
            var staff = await _staffService.GetStaffMembersByTypeAsync(staffType);
            return Ok(ApiResponse<List<StaffMemberDto>>.SuccessResponse(
                staff,
                $"{staffType} staff members retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<StaffMemberDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Create new staff member
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateStaffMember([FromBody] CreateStaffMemberDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _staffService.CreateStaffMemberAsync(dto, userId);

            return Ok(ApiResponse<StaffMemberDto>.SuccessResponse(
                result,
                "Staff member created successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffMemberDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Update staff member details
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateStaffMember([FromBody] UpdateStaffMemberDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _staffService.UpdateStaffMemberAsync(dto, userId);

            return Ok(ApiResponse<StaffMemberDto>.SuccessResponse(
                result,
                "Staff member updated successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffMemberDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Deactivate staff member
    /// </summary>
    [HttpPost("{staffId}/deactivate")]
    public async Task<IActionResult> DeactivateStaffMember(Guid staffId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _staffService.DeactivateStaffMemberAsync(staffId, userId);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Staff member deactivated successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Activate staff member
    /// </summary>
    [HttpPost("{staffId}/activate")]
    public async Task<IActionResult> ActivateStaffMember(Guid staffId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _staffService.ActivateStaffMemberAsync(staffId, userId);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Staff member activated successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get staff member by ID
    /// </summary>
    [HttpGet("{staffId}")]
    public async Task<IActionResult> GetStaffMember(Guid staffId)
    {
        try
        {
            var staff = await _staffService.GetStaffMemberByIdAsync(staffId);

            if (staff == null)
            {
                return NotFound(ApiResponse<StaffMemberDto>.ErrorResponse("Staff member not found"));
            }

            return Ok(ApiResponse<StaffMemberDto>.SuccessResponse(
                staff,
                "Staff member retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffMemberDto>.ErrorResponse(ex.Message));
        }
    }
}