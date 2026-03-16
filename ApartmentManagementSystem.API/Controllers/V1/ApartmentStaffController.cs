using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1;

/// <summary>
/// REST API controller for staff member management.
/// Exposes endpoints for retrieving, creating, updating, activating,
/// and deactivating staff members within the apartment management system.
/// Accessible to SuperAdmin, Manager, President, Secretary, and Treasurer roles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = SystemRoles.AdminManagerCommunity)]
public class ApartmentStaffController : ControllerBase
{
    private readonly IStaffMemberService StaffService;

    /// <summary>
    /// Injects the staff member service used for all business logic operations.
    /// </summary>
    public ApartmentStaffController(IStaffMemberService staffService)
    {
        StaffService = staffService;
    }

    /// <summary>
    /// Retrieves all staff members across all apartments.
    /// Returns a flat list including apartment name for each staff member.
    /// </summary>
    /// <returns>List of all staff member DTOs with apartment info.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllStaffMembers()
    {
        try
        {
            var staff = await StaffService.GetAllStaffMembersAsync();
            return Ok(ApiResponse<List<StaffMemberDto>>.SuccessResponse(staff, StaffMessages.RetrievedAll));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<StaffMemberDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Retrieves all staff members of a specific type
    /// (e.g., Security, Plumber, Electrician, Carpenter, Sweeper, Gardener, MaintenanceStaff).
    /// </summary>
    /// <param name="staffType">The staff type string to filter by.</param>
    /// <returns>Filtered list of staff member DTOs matching the given type.</returns>
    [HttpGet("by-type/{staffType}")]
    public async Task<IActionResult> GetStaffMembersByType(string staffType)
    {
        try
        {
            var staff = await StaffService.GetStaffMembersByTypeAsync(staffType);
            return Ok(ApiResponse<List<StaffMemberDto>>.SuccessResponse(
                staff, string.Format(StaffMessages.RetrievedByType, staffType)));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<StaffMemberDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Retrieves a single staff member by their unique identifier.
    /// Returns 404 Not Found if no matching staff member exists.
    /// </summary>
    /// <param name="staffId">The GUID of the staff member to retrieve.</param>
    /// <returns>Staff member DTO if found; 404 response otherwise.</returns>
    [HttpGet("{staffId}")]
    public async Task<IActionResult> GetStaffMember(Guid staffId)
    {
        try
        {
            var staff = await StaffService.GetStaffMemberByIdAsync(staffId);
            if (staff == null)
                return NotFound(ApiResponse<StaffMemberDto>.ErrorResponse(StaffMessages.NotFound));

            return Ok(ApiResponse<StaffMemberDto>.SuccessResponse(staff, StaffMessages.RetrievedById));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffMemberDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Creates a new staff member and optionally assigns them to an apartment.
    /// If login access is requested and a password is provided, a user account
    /// is also created for the staff member with the appropriate role.
    /// The currently authenticated user is recorded as the creator.
    /// </summary>
    /// <param name="dto">
    /// Staff member creation data including name, contact info, type,
    /// optional apartment assignment, and optional login credentials.
    /// </param>
    /// <returns>The newly created staff member DTO.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateStaffMember([FromBody] CreateStaffMemberDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.CreateStaffMemberAsync(dto, userId);
            return Ok(ApiResponse<StaffMemberDto>.SuccessResponse(result, StaffMessages.Created));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffMemberDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Updates an existing staff member's details including name, contact info,
    /// specialization, hourly rate, active status, and apartment assignment.
    /// If the staff member has a linked user account, that account's name,
    /// phone, and email are also updated to stay in sync.
    /// The currently authenticated user is recorded as the updater.
    /// </summary>
    /// <param name="dto">The updated staff member data.</param>
    /// <returns>The updated staff member DTO.</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateStaffMember([FromBody] UpdateStaffMemberDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.UpdateStaffMemberAsync(dto, userId);
            return Ok(ApiResponse<StaffMemberDto>.SuccessResponse(result, StaffMessages.Updated));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffMemberDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Deactivates a staff member by setting their IsActive flag to false.
    /// Deactivated staff members will not appear as active in dashboards
    /// and cannot log in if they have a linked user account.
    /// The currently authenticated user is recorded as the one who performed the action.
    /// </summary>
    /// <param name="staffId">The GUID of the staff member to deactivate.</param>
    /// <returns>True if deactivation was successful.</returns>
    [HttpPost("{staffId}/deactivate")]
    public async Task<IActionResult> DeactivateStaffMember(Guid staffId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.DeactivateStaffMemberAsync(staffId, userId);
            return Ok(ApiResponse<bool>.SuccessResponse(result, StaffMessages.Deactivated));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Reactivates a previously deactivated staff member by setting their IsActive flag to true.
    /// Restores their active status in dashboards and re-enables login if applicable.
    /// The currently authenticated user is recorded as the one who performed the action.
    /// </summary>
    /// <param name="staffId">The GUID of the staff member to activate.</param>
    /// <returns>True if activation was successful.</returns>
    [HttpPost("{staffId}/activate")]
    public async Task<IActionResult> ActivateStaffMember(Guid staffId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await StaffService.ActivateStaffMemberAsync(staffId, userId);
            return Ok(ApiResponse<bool>.SuccessResponse(result, StaffMessages.Activated));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }
}
