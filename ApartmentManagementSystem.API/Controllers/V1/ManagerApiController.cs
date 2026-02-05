
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces.Services;
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

        /// <summary>
        /// Get ResidentOwners from this apartment who can be assigned as manager
        /// </summary>
        [HttpGet("apartment-residents/{apartmentId}")]
        public async Task<IActionResult> GetApartmentResidents(Guid apartmentId)
        {
            try
            {
                var residents = await ManagerService.GetApartmentResidentsForManagerAssignmentAsync(apartmentId);

                return Ok(ApiResponse<List<AvailableManagerDto>>.SuccessResponse(
                    residents,
                    "Residents retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<AvailableManagerDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Assign manager - supports both resident and external person
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerRequestDto dto)
        {
            try
            {
                var assignedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await ManagerService.AssignManagerToApartmentAsync(dto, assignedByUserId);

                return Ok(ApiResponse<ManagerAssignmentDto>.SuccessResponse(
                    result,
                    "Manager assigned successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ManagerAssignmentDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Remove manager from apartment
        /// </summary>
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveManager([FromBody] RemoveManagerRequestDto dto)
        {
            try
            {
                var removedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await ManagerService.RemoveManagerFromApartmentAsync(dto, removedByUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Manager removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}


