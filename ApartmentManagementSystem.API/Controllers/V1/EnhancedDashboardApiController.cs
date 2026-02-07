using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
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

    /// Get manager dashboard
    [HttpGet("manager")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetManagerDashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dashboard = await DashboardService.GetManagerDashboardAsync(userId);

            return Ok(ApiResponse<ManagerDashboardDto>.SuccessResponse(
                dashboard,
                "Manager dashboard loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ManagerDashboardDto>.ErrorResponse(ex.Message));
        }
    }

    /// Get community leader dashboard (President, Secretary, Treasurer)
    [HttpGet("community-leader")]
    [Authorize(Roles = "President,Secretary,Treasurer")]
    public async Task<IActionResult> GetCommunityLeaderDashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
            var dashboard = await DashboardService.GetCommunityLeaderDashboardAsync(userId, role);

            return Ok(ApiResponse<CommunityLeaderDashboardDto>.SuccessResponse(
                dashboard,
                $"{role} dashboard loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityLeaderDashboardDto>.ErrorResponse(ex.Message));
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

    /// Get apartment statistics
    [HttpGet("apartment-stats/{apartmentId}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetApartmentStats(Guid apartmentId)
    {
        try
        {
            var stats = await DashboardService.GetApartmentDashboardStatsAsync(apartmentId);

            return Ok(ApiResponse<ApartmentDashboardStatsDto>.SuccessResponse(
                stats,
                "Apartment statistics loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ApartmentDashboardStatsDto>.ErrorResponse(ex.Message));
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

    /// Get apartment financial summary
    [HttpGet("apartment-financial-summary/{apartmentId}")]
    [Authorize(Roles = "SuperAdmin,Manager,Treasurer")]
    public async Task<IActionResult> GetApartmentFinancialSummary(Guid apartmentId)
    {
        try
        {
            var summary = await DashboardService.GetApartmentFinancialSummaryAsync(apartmentId);

            return Ok(ApiResponse<FinancialSummaryDto>.SuccessResponse(
                summary,
                "Apartment financial summary loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<FinancialSummaryDto>.ErrorResponse(ex.Message));
        }
    }

    /// Get notice board messages
    [HttpGet("notice-board/{apartmentId}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetNoticeBoardMessages(Guid apartmentId)
    {
        try
        {
            var messages = await DashboardService.GetNoticeBoardMessagesAsync(apartmentId);

            return Ok(ApiResponse<List<NoticeBoardMessageDto>>.SuccessResponse(
                messages,
                "Notice board messages loaded successfully"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<NoticeBoardMessageDto>>.ErrorResponse(ex.Message));
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









/*using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1;

[ApiController]
//[ApiVersion("1.0")]  
//[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
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
*/