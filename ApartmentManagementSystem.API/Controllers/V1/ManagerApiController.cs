using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController] 
    [ApiVersion("1.0")]  
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class ManagerApiController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerApiController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        /// <summary>
        /// Get available managers who can be assigned to an apartment
        /// </summary>
        [HttpGet("available/{apartmentId}")]
        public async Task<IActionResult> GetAvailableManagers(Guid apartmentId)
        {
            try
            {
                var managers = await _managerService.GetAvailableManagersAsync(apartmentId);
                return Ok(ApiResponse<List<AvailableManagerDto>>.SuccessResponse(
                    managers,
                    "Available managers retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<AvailableManagerDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Assign a manager to an apartment
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerRequestDto dto)
        {
            try
            {
                var assignedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _managerService.AssignManagerToApartmentAsync(dto, assignedByUserId);

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
        /// Remove manager from an apartment
        /// </summary>
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveManager([FromBody] RemoveManagerRequestDto dto)
        {
            try
            {
                var removedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _managerService.RemoveManagerFromApartmentAsync(dto, removedByUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    "Manager removed successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get all managers in the system
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllManagers()
        {
            try
            {
                var managers = await _managerService.GetAllManagersAsync();
                return Ok(ApiResponse<List<ManagerListDto>>.SuccessResponse(
                    managers,
                    "Managers retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ManagerListDto>>.ErrorResponse(ex.Message));
            }
        }
    }
}