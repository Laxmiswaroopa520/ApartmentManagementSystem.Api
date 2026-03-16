using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    /// <summary>
    /// Handles resident management operations such as:
    /// - Viewing residents
    /// - Filtering residents by type (Owner/Tenant)
    /// - Viewing detailed resident information
    /// - Activating and deactivating resident accounts
    /// 
    /// All endpoints require authentication.
    /// Role-based restrictions are applied per endpoint.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResidentManagementController : ControllerBase
    {
        /// <summary>
        /// Service responsible for resident management business logic.
        /// </summary>
        private readonly IResidentManagementService ResidentService;

        /// <summary>
        /// Constructor for injecting resident management service.
        /// </summary>
        /// <param name="residentService">
        /// Service that handles resident retrieval, activation, and deactivation.
        /// </param>
        public ResidentManagementController(IResidentManagementService residentService)
        {
            ResidentService = residentService;
        }

        /// <summary>
        /// Retrieves all residents in the system.
        /// 
        /// Accessible Roles:
        /// - SuperAdmin
        /// - Manager
        /// - President
        /// - Secretary
        /// - Treasurer
        /// </summary>
        /// <returns>
        /// List of residents.
        /// </returns>
        [HttpGet]
        [Authorize(Roles =SystemRoles.AdminManagerCommunity)]
        public async Task<IActionResult> GetAllResidents()
        {
            try
            {
                var residents = await ResidentService.GetAllResidentsAsync();

                return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                    residents,
                    ResidentMessages.ResidentsRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ResidentListDto>>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves residents filtered by type (Owner or Tenant).
        /// </summary>
        /// <param name="residentType">
        /// Type of resident (e.g., "Owner", "Tenant").
        /// </param>
        /// <returns>
        /// List of residents matching the specified type.
        /// </returns>
        [HttpGet("by-type/{residentType}")]
        [Authorize(Roles = SystemRoles.AdminManagerCommunity)]

        public async Task<IActionResult> GetResidentsByType(string residentType)
        {
            try
            {
                var residents = await ResidentService
                    .GetResidentsByTypeAsync(residentType);

                return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                    residents,
                    $"{residentType} {ResidentMessages.ResidentsByTypeRetrievedSuffix}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ResidentListDto>>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves detailed information for a specific resident.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the resident.
        /// </param>
        /// <returns>
        /// Detailed resident information.
        /// </returns>
        [HttpGet("{userId}")]
        [Authorize(Roles = SystemRoles.AdminManagerCommunity)]
        public async Task<IActionResult> GetResidentDetail(Guid userId)
        {
            try
            {
                var resident = await ResidentService
                    .GetResidentDetailAsync(userId);

                if (resident == null)
                {
                    return NotFound(ApiResponse<ResidentDetailDto>
                        .ErrorResponse(ResidentMessages.ResidentNotFound));
                }

                return Ok(ApiResponse<ResidentDetailDto>.SuccessResponse(
                    resident,
                    ResidentMessages.ResidentDetailsRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ResidentDetailDto>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Deactivates a resident account.
        /// 
        /// Accessible Roles:
        /// - SuperAdmin
        /// - Manager
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the resident to deactivate.
        /// </param>
        /// <returns>
        /// Boolean indicating whether deactivation was successful.
        /// </returns>
        [HttpPost("{userId}/deactivate")]
        [Authorize(Roles = SystemRoles.SuperAdmin+","+SystemRoles.Manager)]
        public async Task<IActionResult> DeactivateResident(Guid userId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var currentUserId = Guid.Parse(userIdClaim);

                var result = await ResidentService
                    .DeactivateResidentAsync(userId, currentUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    ResidentMessages.ResidentDeactivated
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Activates a previously deactivated resident account.
        /// 
        /// Accessible Roles:
        /// - SuperAdmin
        /// - Manager
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the resident to activate.
        /// </param>
        /// <returns>
        /// Boolean indicating whether activation was successful.
        /// </returns>
        [HttpPost("{userId}/activate")]
        [Authorize(Roles = SystemRoles.SuperAdmin + "," + SystemRoles.Manager)]
        public async Task<IActionResult> ActivateResident(Guid userId)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var currentUserId = Guid.Parse(userIdClaim);

                var result = await ResidentService
                    .ActivateResidentAsync(userId, currentUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    ResidentMessages.ResidentActivated
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>
                    .ErrorResponse(ex.Message));
            }
        }
    }
}




















































