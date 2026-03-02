using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    /// <summary>
    /// Handles apartment manager management operations.
    /// 
    /// This controller allows:
    /// - Fetching eligible apartment residents for manager assignment
    /// - Assigning a manager (resident or external person)
    /// - Removing a manager from an apartment
    /// 
    /// Access restricted to SuperAdmin role only.
    /// </summary>
    [ApiController]
    [Route("api/Manager")]
    [Authorize(Roles = "SuperAdmin")]
    public class ManagerApiController : ControllerBase
    {
        /// <summary>
        /// Service responsible for manager-related business logic.
        /// </summary>
        private readonly IManagerService ManagerService;

        /// <summary>
        /// Constructor for injecting manager service.
        /// </summary>
        /// <param name="managerService">
        /// Service that handles manager assignment and removal operations.
        /// </param>
        public ManagerApiController(IManagerService managerService)
        {
            ManagerService = managerService;
        }

        /// <summary>
        /// Retrieves Resident Owners from a specific apartment
        /// who are eligible to be assigned as managers.
        /// </summary>
        /// <param name="apartmentId">
        /// Unique identifier of the apartment.
        /// </param>
        /// <returns>
        /// List of available residents eligible for manager assignment.
        /// </returns>
        [HttpGet("apartment-residents/{apartmentId}")]
        public async Task<IActionResult> GetApartmentResidents(Guid apartmentId)
        {
            try
            {
                var residents = await ManagerService
                    .GetApartmentResidentsForManagerAssignmentAsync(apartmentId);

                return Ok(ApiResponse<List<AvailableManagerDto>>.SuccessResponse(
                    residents,
                    ManagerMessages.ResidentsRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<AvailableManagerDto>>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Assigns a manager to an apartment.
        /// 
        /// Supports:
        /// - Assigning an existing resident owner
        /// - Assigning an external person as manager
        /// </summary>
        /// <param name="dto">
        /// Contains manager assignment details such as ApartmentId and Manager information.
        /// </param>
        /// <returns>
        /// Manager assignment details.
        /// </returns>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignManager(
            [FromBody] AssignManagerRequestDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var assignedByUserId = Guid.Parse(userIdClaim);

                var result = await ManagerService
                    .AssignManagerToApartmentAsync(dto, assignedByUserId);

                return Ok(ApiResponse<ManagerAssignmentDto>.SuccessResponse(
                    result,
                    ManagerMessages.ManagerAssigned
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ManagerAssignmentDto>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Removes an assigned manager from an apartment.
        /// </summary>
        /// <param name="dto">
        /// Contains details required to remove the manager.
        /// </param>
        /// <returns>
        /// Boolean indicating whether removal was successful.
        /// </returns>
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveManager(
            [FromBody] RemoveManagerRequestDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                var removedByUserId = Guid.Parse(userIdClaim);

                var result = await ManagerService
                    .RemoveManagerFromApartmentAsync(dto, removedByUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    ManagerMessages.ManagerRemoved
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




















/*using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController]
    [Route("api/Manager")]
    [Authorize(Roles = "SuperAdmin")]
    public class ManagerApiController : ControllerBase
    {
        private readonly IManagerService ManagerService;

        public ManagerApiController(IManagerService managerService)
        {
            ManagerService = managerService;
        }

        // Get ResidentOwners from this apartment who can be assigned as manager
        [HttpGet("apartment-residents/{apartmentId}")]
        public async Task<IActionResult> GetApartmentResidents(Guid apartmentId)
        {
            try
            {
                var residents = await ManagerService
                    .GetApartmentResidentsForManagerAssignmentAsync(apartmentId);

                return Ok(ApiResponse<List<AvailableManagerDto>>.SuccessResponse(
                    residents,
                    ManagerMessages.ResidentsRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<AvailableManagerDto>>
                    .ErrorResponse(ex.Message));
            }
        }

        // Assign manager - supports both resident and external person
        [HttpPost("assign")]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerRequestDto dto)
        {
            try
            {
                var assignedByUserId =
                    Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await ManagerService
                    .AssignManagerToApartmentAsync(dto, assignedByUserId);

                return Ok(ApiResponse<ManagerAssignmentDto>.SuccessResponse(
                    result,
                    ManagerMessages.ManagerAssigned
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ManagerAssignmentDto>
                    .ErrorResponse(ex.Message));
            }
        }

        // Remove manager from apartment
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveManager(
            [FromBody] RemoveManagerRequestDto dto)
        {
            try
            {
                var removedByUserId =
                    Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await ManagerService
                    .RemoveManagerFromApartmentAsync(dto, removedByUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    ManagerMessages.ManagerRemoved
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

*/


























