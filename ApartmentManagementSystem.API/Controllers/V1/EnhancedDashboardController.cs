using Microsoft.AspNetCore.Mvc;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnhancedDashboardController : ControllerBase
    {
        private readonly IEnhancedDashboardService DashboardService;

        /// <summary>
        /// Injects the enhanced dashboard service that handles all
        /// dashboard data assembly and business logic.
        /// </summary>
        public EnhancedDashboardController(IEnhancedDashboardService dashboardService)
        {
            DashboardService = dashboardService;
        }

        /// <summary>
        /// Returns the enhanced admin dashboard payload for SuperAdmin and management roles.
        /// Includes portfolio overview, key metrics (residents, flats, staff, community members),
        /// financial summary, module statuses, and recent activity log.
        /// The calling user's ID is used to scope data where applicable.
        /// </summary>
        /// <returns>Fully assembled enhanced admin dashboard DTO.</returns>
        [HttpGet("admin")]
        [Authorize(Roles =SystemRoles.AdminManagerCommunity)]
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

        /// <summary>
        /// Returns the manager-specific dashboard payload.
        /// Scoped to the apartments assigned to the calling manager.
        /// Includes occupancy stats, pending registrations, and staff overview
        /// for their managed properties.
        /// </summary>
        /// <returns>Manager dashboard DTO scoped to the calling user's apartments.</returns>
        [HttpGet("manager")]
        [Authorize(Roles = SystemRoles.Manager)]
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

        /// <summary>
        /// Returns the community leader dashboard for President, Secretary, or Treasurer roles.
        /// The active community role is resolved from the user's claims and passed to the service
        /// to tailor the dashboard content accordingly (e.g., financial data for Treasurer).
        /// Returns 400 if no valid community leader role is found in the claims.
        /// </summary>
        /// <returns>Community leader dashboard DTO tailored to the user's community role.</returns>
        [HttpGet("community-leader")]
        [Authorize(Roles =SystemRoles.President+","+SystemRoles.Secretary+","+SystemRoles.Treasurer)]
        public async Task<IActionResult> GetCommunityLeaderDashboard()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
                var role = roles.FirstOrDefault(r =>
                    r == SystemRoles.President || r == SystemRoles.Secretary || r == SystemRoles.Treasurer) ?? string.Empty;

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

        /// <summary>
        /// Returns the staff member dashboard for operational roles
        /// (Security, Plumber, Electrician, Carpenter, Sweeper, Gardener, MaintenanceStaff).
        /// Scoped to the calling staff member's assigned apartment and work history.
        /// </summary>
        /// <returns>Staff dashboard DTO for the calling staff user.</returns>
        [HttpGet("staff")]
        [Authorize(Roles =SystemRoles.StaffRolesString)]
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

        /// <summary>
        /// Returns system-wide advanced dashboard statistics for management roles.
        /// Includes aggregated data such as total occupancy rates, staff distribution,
        /// complaint counts, and billing summaries across all apartments.
        /// Not scoped to any specific apartment or user.
        /// </summary>
        /// <returns>Advanced dashboard stats DTO with system-wide aggregates.</returns>
        [HttpGet("advanced-stats")]
        [Authorize(Roles =SystemRoles.AdminManagerCommunity)]
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

        /// <summary>
        /// Returns dashboard statistics scoped to a single apartment.
        /// Includes floor/flat occupancy breakdown, assigned manager info,
        /// community member roles, and staff count for that building.
        /// </summary>
        /// <param name="apartmentId">The GUID of the apartment to retrieve stats for.</param>
        /// <returns>Apartment-scoped dashboard stats DTO.</returns>
        [HttpGet("apartment-stats/{apartmentId}")]
        [Authorize(Roles = SystemRoles.AdminManagerCommunity)]

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

        /// <summary>
        /// Returns the financial summary for the entire system.
        /// Includes total outstanding maintenance dues and the amount collected
        /// in the current month across all apartments.
        /// Restricted to SuperAdmin and Treasurer roles only.
        /// </summary>
        /// <returns>System-wide financial summary DTO.</returns>
        [HttpGet("financial-summary")]
        [Authorize(Roles =SystemRoles.SuperAdmin+","+SystemRoles.Treasurer)]
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

        /// <summary>
        /// Returns the financial summary scoped to a specific apartment.
        /// Useful for Managers and Treasurers who need per-building billing insight.
        /// </summary>
        /// <param name="apartmentId">The GUID of the apartment to retrieve financial data for.</param>
        /// <returns>Apartment-scoped financial summary DTO.</returns>
        [HttpGet("apartment-financial-summary/{apartmentId}")]
        [Authorize(Roles = SystemRoles.SuperAdmin + "," + SystemRoles.Treasurer+","+SystemRoles.Manager)]

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

        /// <summary>
        /// Returns the notice board messages for a specific apartment.
        /// Used to display community announcements, maintenance alerts,
        /// and important updates on the dashboard notice board section.
        /// </summary>
        /// <param name="apartmentId">The GUID of the apartment whose notice board to fetch.</param>
        /// <returns>List of notice board message DTOs for the given apartment.</returns>
        [HttpGet("notice-board/{apartmentId}")]
        [Authorize(Roles = SystemRoles.AdminManagerCommunity)]
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

        /// <summary>
        /// Returns the list of quick action tiles relevant to the calling user's role.
        /// Resolves the highest-priority role from the user's claims (SuperAdmin > Manager > Community Leader)
        /// and delegates to the service to return the appropriate action set.
        /// Used to populate the Quick Actions section on the dashboard.
        /// </summary>
        /// <returns>List of quick action DTOs tailored to the calling user's role.</returns>
        [HttpGet("quick-actions")]
        public async Task<IActionResult> GetQuickActions()
        {
            try
            {
                var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

                var role = roles.Contains(SystemRoles.SuperAdmin) ? SystemRoles.SuperAdmin :
                           roles.Contains(SystemRoles.Manager) ? SystemRoles.Manager :
                           roles.FirstOrDefault(r => r == SystemRoles.President || r == SystemRoles.Secretary || r == SystemRoles.Treasurer) ??
                           roles.FirstOrDefault() ?? string.Empty;

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
}