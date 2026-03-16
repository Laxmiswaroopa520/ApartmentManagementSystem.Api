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
    public class DashboardController : ControllerBase
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
        public DashboardController(IDashboardService dashboardService)
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
        [Authorize(Roles = SystemRoles.AdminManagerCommunity)]

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
        // [Authorize(Roles = "ResidentOwner")]
        [Authorize(Roles = SystemRoles.ResidentOwner)]

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
        //  [Authorize(Roles = "Tenant")]
        [Authorize(Roles = SystemRoles.Tenant)]

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
        //  [Authorize(Roles = "SuperAdmin,President,Secretary,Treasurer")]
        [Authorize(Roles = SystemRoles.SuperAdmin + "," + SystemRoles.Manager + "," + SystemRoles.President + "," + SystemRoles.Secretary + "," + SystemRoles.Treasurer)]
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





































































