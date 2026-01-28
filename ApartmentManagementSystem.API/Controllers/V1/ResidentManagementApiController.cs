using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ApartmentManagementSystem.API.Controllers.V1;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResidentManagementApiController : ControllerBase
{
    private readonly IResidentManagementService ResidentService;

    public ResidentManagementApiController(IResidentManagementService residentService)
    {
        ResidentService = residentService;
    }
    /// Get all residents (Admin, President, Secretary, Treasurer can view)

    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetAllResidents()
    {
        try
        {
            var residents = await ResidentService.GetAllResidentsAsync();
            return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                residents,
                "Residents retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ResidentListDto>>.ErrorResponse(ex.Message));
        }
    }
    /// Get residents by type (Owner/Tenant)
    [HttpGet("by-type/{residentType}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetResidentsByType(string residentType)
    {
        try
        {
            var residents = await ResidentService.GetResidentsByTypeAsync(residentType);
            return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                residents,
                $"{residentType} residents retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ResidentListDto>>.ErrorResponse(ex.Message));
        }
    }
    /// Get detailed resident information
    [HttpGet("{userId}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetResidentDetail(Guid userId)
    {
        try
        {
            var resident = await ResidentService.GetResidentDetailAsync(userId);

            if (resident == null)
            {
                return NotFound(ApiResponse<ResidentDetailDto>.ErrorResponse("Resident not found"));
            }

            return Ok(ApiResponse<ResidentDetailDto>.SuccessResponse(
                resident,
                "Resident details retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ResidentDetailDto>.ErrorResponse(ex.Message));
        }
    }

    /// Deactivate resident account
    [HttpPost("{userId}/deactivate")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> DeactivateResident(Guid userId)
    {
        try
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await ResidentService.DeactivateResidentAsync(userId, currentUserId);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Resident deactivated successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }
    /// Activate resident account
    [HttpPost("{userId}/activate")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> ActivateResident(Guid userId)
    {
        try
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await ResidentService.ActivateResidentAsync(userId, currentUserId);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result,
                "Resident activated successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }
}