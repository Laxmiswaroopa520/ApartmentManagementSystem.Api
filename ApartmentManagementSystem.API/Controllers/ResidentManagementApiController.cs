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
[Authorize]
public class ResidentManagementApiController : ControllerBase
{
    private readonly IResidentManagementService _residentService;

    public ResidentManagementApiController(IResidentManagementService residentService)
    {
        _residentService = residentService;
    }

    /// <summary>
    /// Get all residents (Admin, President, Secretary, Treasurer can view)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetAllResidents()
    {
        try
        {
            var residents = await _residentService.GetAllResidentsAsync();
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

    /// <summary>
    /// Get residents by type (Owner/Tenant)
    /// </summary>
    [HttpGet("by-type/{residentType}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetResidentsByType(string residentType)
    {
        try
        {
            var residents = await _residentService.GetResidentsByTypeAsync(residentType);
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

    /// <summary>
    /// Get detailed resident information
    /// </summary>
    [HttpGet("{userId}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetResidentDetail(Guid userId)
    {
        try
        {
            var resident = await _residentService.GetResidentDetailAsync(userId);

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

    /// <summary>
    /// Deactivate resident account
    /// </summary>
    [HttpPost("{userId}/deactivate")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> DeactivateResident(Guid userId)
    {
        try
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _residentService.DeactivateResidentAsync(userId, currentUserId);

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

    /// <summary>
    /// Activate resident account
    /// </summary>
    [HttpPost("{userId}/activate")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public async Task<IActionResult> ActivateResident(Guid userId)
    {
        try
        {
            var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _residentService.ActivateResidentAsync(userId, currentUserId);

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