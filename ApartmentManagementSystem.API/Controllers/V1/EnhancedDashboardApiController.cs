using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
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
                EnhancedDashboardMessages.EnhancedAdminDashboardLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<EnhancedAdminDashboardDto>.ErrorResponse(ex.Message));
        }
    }

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
                EnhancedDashboardMessages.ManagerDashboardLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ManagerDashboardDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("community-leader")]
    [Authorize(Roles = "President,Secretary,Treasurer")]
    public async Task<IActionResult> GetCommunityLeaderDashboard()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            var role = roles.FirstOrDefault(r =>
                r == "President" || r == "Secretary" || r == "Treasurer") ?? "";

            if (string.IsNullOrEmpty(role))
            {
                return BadRequest(ApiResponse<CommunityLeaderDashboardDto>.ErrorResponse(
                    EnhancedDashboardMessages.CommunityLeaderRoleMissing
                ));
            }

            var dashboard = await DashboardService.GetCommunityLeaderDashboardAsync(userId, role);

            return Ok(ApiResponse<CommunityLeaderDashboardDto>.SuccessResponse(
                dashboard,
                $"{role} {EnhancedDashboardMessages.CommunityLeaderDashboardLoaded}"
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CommunityLeaderDashboardDto>.ErrorResponse(ex.Message));
        }
    }

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
                EnhancedDashboardMessages.StaffDashboardLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<StaffDashboardDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("advanced-stats")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetAdvancedDashboardStats()
    {
        try
        {
            var stats = await DashboardService.GetAdvancedDashboardStatsAsync();

            return Ok(ApiResponse<AdvancedDashboardStatsDto>.SuccessResponse(
                stats,
                EnhancedDashboardMessages.AdvancedStatsLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AdvancedDashboardStatsDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("apartment-stats/{apartmentId}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetApartmentStats(Guid apartmentId)
    {
        try
        {
            var stats = await DashboardService.GetApartmentDashboardStatsAsync(apartmentId);

            return Ok(ApiResponse<ApartmentDashboardStatsDto>.SuccessResponse(
                stats,
                EnhancedDashboardMessages.ApartmentStatsLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ApartmentDashboardStatsDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("financial-summary")]
    [Authorize(Roles = "SuperAdmin,Treasurer")]
    public async Task<IActionResult> GetFinancialSummary()
    {
        try
        {
            var summary = await DashboardService.GetFinancialSummaryAsync();

            return Ok(ApiResponse<FinancialSummaryDto>.SuccessResponse(
                summary,
                EnhancedDashboardMessages.FinancialSummaryLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<FinancialSummaryDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("apartment-financial-summary/{apartmentId}")]
    [Authorize(Roles = "SuperAdmin,Manager,Treasurer")]
    public async Task<IActionResult> GetApartmentFinancialSummary(Guid apartmentId)
    {
        try
        {
            var summary = await DashboardService.GetApartmentFinancialSummaryAsync(apartmentId);

            return Ok(ApiResponse<FinancialSummaryDto>.SuccessResponse(
                summary,
                EnhancedDashboardMessages.ApartmentFinancialSummaryLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<FinancialSummaryDto>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("notice-board/{apartmentId}")]
    [Authorize(Roles = "SuperAdmin,Manager,President,Secretary,Treasurer")]
    public async Task<IActionResult> GetNoticeBoardMessages(Guid apartmentId)
    {
        try
        {
            var messages = await DashboardService.GetNoticeBoardMessagesAsync(apartmentId);

            return Ok(ApiResponse<List<NoticeBoardMessageDto>>.SuccessResponse(
                messages,
                EnhancedDashboardMessages.NoticeBoardMessagesLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<NoticeBoardMessageDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("quick-actions")]
    public async Task<IActionResult> GetQuickActions()
    {
        try
        {
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            var role = roles.Contains("SuperAdmin") ? "SuperAdmin" :
                       roles.Contains("Manager") ? "Manager" :
                       roles.FirstOrDefault(r => r == "President" || r == "Secretary" || r == "Treasurer") ??
                       roles.FirstOrDefault() ?? "";

            var actions = await DashboardService.GetQuickActionsForRoleAsync(role);

            return Ok(ApiResponse<List<QuickActionDto>>.SuccessResponse(
                actions,
                EnhancedDashboardMessages.QuickActionsLoaded
            ));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<QuickActionDto>>.ErrorResponse(ex.Message));
        }
    }
}





























