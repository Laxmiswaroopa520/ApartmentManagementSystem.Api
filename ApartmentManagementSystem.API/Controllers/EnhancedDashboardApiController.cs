using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnhancedDashboardApiController : ControllerBase
{
    private readonly IEnhancedDashboardService _dashboardService;

    public EnhancedDashboardApiController(IEnhancedDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Get enhanced admin dashboard with all features
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetEnhancedAdminDashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dashboard = await _dashboardService.GetEnhancedAdminDashboardAsync(userId);

            return Ok(ApiResponse<EnhancedAdminDashboardDto>.SuccessResponse(
                dashboard,
                "Enhanced admin dashboard loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<EnhancedAdminDashboardDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get staff member dashboard (minimal)
    /// </summary>
    [HttpGet("staff")]
    [Authorize(Roles = "Security,Plumber,Electrician,Carpenter,Sweeper,Gardener,MaintenanceStaff")]
    public async Task<IActionResult> GetStaffDashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dashboard = await _dashboardService.GetStaffDashboardAsync(userId);

            return Ok(ApiResponse<StaffDashboardDto>.SuccessResponse(
                dashboard,
                "Staff dashboard loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffDashboardDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get advanced statistics for admin dashboard
    /// </summary>
    [HttpGet("advanced-stats")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetAdvancedDashboardStats()
    {
        try
        {
            var stats = await _dashboardService.GetAdvancedDashboardStatsAsync();

            return Ok(ApiResponse<AdvancedDashboardStatsDto>.SuccessResponse(
                stats,
                "Advanced statistics loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AdvancedDashboardStatsDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get financial summary (for Treasurer and SuperAdmin)
    /// </summary>
    [HttpGet("financial-summary")]
    [Authorize(Roles = "SuperAdmin,Treasurer")]
    public async Task<IActionResult> GetFinancialSummary()
    {
        try
        {
            var summary = await _dashboardService.GetFinancialSummaryAsync();

            return Ok(ApiResponse<FinancialSummaryDto>.SuccessResponse(
                summary,
                "Financial summary loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<FinancialSummaryDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Get quick actions based on user role
    /// </summary>
    [HttpGet("quick-actions")]
    public async Task<IActionResult> GetQuickActions()
    {
        try
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
            var actions = await _dashboardService.GetQuickActionsForRoleAsync(role);

            return Ok(ApiResponse<List<QuickActionDto>>.SuccessResponse(
                actions,
                "Quick actions loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<QuickActionDto>>.ErrorResponse(ex.Message));
        }
    }
}