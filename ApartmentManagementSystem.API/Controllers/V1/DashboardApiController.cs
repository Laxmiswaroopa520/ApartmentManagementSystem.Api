using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    /// <summary>
    /// Provides dashboard data for different user roles in the system.
    /// 
    /// This controller handles:
    /// - Admin dashboard
    /// - Owner dashboard
    /// - Tenant dashboard
    /// - Dashboard statistics
    /// 
    /// All endpoints require authenticated users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardApiController : ControllerBase
    {
        /// <summary>
        /// Service responsible for dashboard-related business logic.
        /// </summary>
        private readonly IDashboardService DashBoardService;

        /// <summary>
        /// Constructor for injecting dashboard service.
        /// </summary>
        /// <param name="dashboardService">
        /// Service that provides dashboard data for different roles.
        /// </param>
        public DashboardApiController(IDashboardService dashboardService)
        {
            DashBoardService = dashboardService;
        }

        /// <summary>
        /// Retrieves dashboard data for administrative roles.
        /// 
        /// Accessible Roles:
        /// - SuperAdmin
        /// - President
        /// - Secretary
        /// - Treasurer
        /// </summary>
        /// <returns>
        /// Admin dashboard data wrapped in ApiResponse.
        /// </returns>
        [HttpGet("admin")]
        [Authorize(Roles = "SuperAdmin,President,Secretary,Treasurer")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                var dashboard = await DashBoardService
                    .GetAdminDashboardAsync(userId);

                return Ok(ApiResponse<AdminDashboardDto>.SuccessResponse(
                    dashboard,
                    DashboardMessages.AdminDashboardLoaded
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<AdminDashboardDto>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves dashboard data for Resident Owner users.
        /// </summary>
        /// <returns>
        /// Owner dashboard information.
        /// </returns>
        [HttpGet("owner")]
        [Authorize(Roles = "ResidentOwner")]
        public async Task<IActionResult> GetOwnerDashboard()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                var dashboard = await DashBoardService
                    .GetOwnerDashboardAsync(userId);

                return Ok(ApiResponse<OwnerDashboardDto>.SuccessResponse(
                    dashboard,
                    DashboardMessages.OwnerDashboardLoaded
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OwnerDashboardDto>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves dashboard data for Tenant users.
        /// </summary>
        /// <returns>
        /// Tenant dashboard information.
        /// </returns>
        [HttpGet("tenant")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> GetTenantDashboard()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                var dashboard = await DashBoardService
                    .GetTenantDashboardAsync(userId);

                return Ok(ApiResponse<TenantDashboardDto>.SuccessResponse(
                    dashboard,
                    DashboardMessages.TenantDashboardLoaded
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TenantDashboardDto>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves overall dashboard statistics.
        /// 
        /// Accessible Roles:
        /// - SuperAdmin
        /// - President
        /// - Secretary
        /// - Treasurer
        /// </summary>
        /// <returns>
        /// Aggregated dashboard statistics.
        /// </returns>
        [HttpGet("stats")]
        [Authorize(Roles = "SuperAdmin,President,Secretary,Treasurer")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await DashBoardService
                    .GetDashboardStatsAsync();

                return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(
                    stats,
                    DashboardMessages.DashboardStatsLoaded
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<DashboardStatsDto>
                    .ErrorResponse(ex.Message));
            }
        }
    }
}





































































