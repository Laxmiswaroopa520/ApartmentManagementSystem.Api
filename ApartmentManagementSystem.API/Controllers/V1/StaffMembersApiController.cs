using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1;

[ApiController]
//[ApiVersion("1.0")]  
//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
public class StaffMembersApiController : ControllerBase
{
    private readonly IStaffMemberService StaffService;

    public StaffMembersApiController(IStaffMemberService staffService)
    {
        StaffService = staffService;
    }

    /// Get all staff members
    [HttpGet]
    public async Task<IActionResult> GetAllStaffMembers()
    {
        try
        {
            var staff = await StaffService.GetAllStaffMembersAsync();
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

    /// Get staff members by type (Plumber, Security, etc.)
    [HttpGet("by-type/{staffType}")]
    public async Task<IActionResult> GetStaffMembersByType(string staffType)
    {
        try
        {
            var staff = await StaffService.GetStaffMembersByTypeAsync(staffType);
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

      /// Create new staff member
    [HttpPost]
    public async Task<IActionResult> CreateStaffMember([FromBody] CreateStaffMemberDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.CreateStaffMemberAsync(dto, userId);

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
    /// Update staff member details
    [HttpPut]
    public async Task<IActionResult> UpdateStaffMember([FromBody] UpdateStaffMemberDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.UpdateStaffMemberAsync(dto, userId);

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

    /// Deactivate staff member
    [HttpPost("{staffId}/deactivate")]
    public async Task<IActionResult> DeactivateStaffMember(Guid staffId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.DeactivateStaffMemberAsync(staffId, userId);

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

   
    /// Activate staff member
   
    [HttpPost("{staffId}/activate")]
    public async Task<IActionResult> ActivateStaffMember(Guid staffId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.ActivateStaffMemberAsync(staffId, userId);

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
    /// Get staff member by ID
    [HttpGet("{staffId}")]
    public async Task<IActionResult> GetStaffMember(Guid staffId)
    {
        try
        {
            var staff = await StaffService.GetStaffMemberByIdAsync(staffId);

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