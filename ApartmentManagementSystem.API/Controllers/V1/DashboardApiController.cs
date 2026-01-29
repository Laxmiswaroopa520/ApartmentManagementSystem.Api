using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")] 
    [Route("api/v{version:apiVersion}/[controller]")]
   // [Route("api/[controller]")]
    [Authorize]
    public class DashboardApiController : ControllerBase
    {
        private readonly IDashboardService DashBoardService;

        public DashboardApiController(IDashboardService dashboardService)
        {
            DashBoardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "SuperAdmin,President,Secretary,Treasurer")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var dashboard = await DashBoardService.GetAdminDashboardAsync(userId);

                return Ok(ApiResponse<AdminDashboardDto>.SuccessResponse(
                    dashboard,
                    "Admin dashboard loaded successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<AdminDashboardDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("owner")]
        [Authorize(Roles = "ResidentOwner")]
        public async Task<IActionResult> GetOwnerDashboard()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var dashboard = await DashBoardService.GetOwnerDashboardAsync(userId);

                return Ok(ApiResponse<OwnerDashboardDto>.SuccessResponse(
                    dashboard,
                    "Owner dashboard loaded successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<OwnerDashboardDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("tenant")]
        [Authorize(Roles = "Tenant")]
        public async Task<IActionResult> GetTenantDashboard()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var dashboard = await DashBoardService.GetTenantDashboardAsync(userId);

                return Ok(ApiResponse<TenantDashboardDto>.SuccessResponse(
                    dashboard,
                    "Tenant dashboard loaded successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TenantDashboardDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("stats")]
        [Authorize(Roles = "SuperAdmin,President,Secretary,Treasurer")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await DashBoardService.GetDashboardStatsAsync();

                return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(
                    stats,
                    "Dashboard statistics loaded successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<DashboardStatsDto>.ErrorResponse(ex.Message));
            }
        }
    }

}
