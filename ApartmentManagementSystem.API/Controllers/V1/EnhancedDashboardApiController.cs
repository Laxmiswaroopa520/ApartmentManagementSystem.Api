using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]  
[Route("api/v{version:apiVersion}/[controller]")]
//[Route("api/[controller]")]
[Authorize]
public class EnhancedDashboardApiController : ControllerBase
{
    private readonly IEnhancedDashboardService DashboardService;

    public EnhancedDashboardApiController(IEnhancedDashboardService dashboardService)
    {
        DashboardService = dashboardService;
    }
    /// Get enhanced admin dashboard with all features
    [HttpGet("admin")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetEnhancedAdminDashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dashboard = await DashboardService.GetEnhancedAdminDashboardAsync(userId);

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
    /// Get staff member dashboard (minimal)
    [HttpGet("staff")]
    [Authorize(Roles = "Security,Plumber,Electrician,Carpenter,Sweeper,Gardener,MaintenanceStaff")]
    public async Task<IActionResult> GetStaffDashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dashboard = await DashboardService.GetStaffDashboardAsync(userId);

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

    /// Get advanced statistics for admin dashboard
    [HttpGet("advanced-stats")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetAdvancedDashboardStats()
    {
        try
        {
            var stats = await DashboardService.GetAdvancedDashboardStatsAsync();

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

    /// Get financial summary (for Treasurer and SuperAdmin)
    [HttpGet("financial-summary")]
    [Authorize(Roles = "SuperAdmin,Treasurer")]
    public async Task<IActionResult> GetFinancialSummary()
    {
        try
        {
            var summary = await DashboardService.GetFinancialSummaryAsync();

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
    /// Get quick actions based on user role
     [HttpGet("quick-actions")]
    public async Task<IActionResult> GetQuickActions()
    {
        try
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
            var actions = await DashboardService.GetQuickActionsForRoleAsync(role);

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