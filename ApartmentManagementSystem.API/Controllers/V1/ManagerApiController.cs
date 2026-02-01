/*

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
        private readonly IManagerService _managerService;

        public ManagerApiController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        /// <summary>
        /// ⭐ NEW CORRECTED: Get ResidentOwners from this apartment (not Manager role users)
        /// Route: GET /api/Manager/apartment-residents/{apartmentId}
        /// </summary>
        [HttpGet("apartment-residents/{apartmentId}")]
        public async Task<IActionResult> GetApartmentResidents(Guid apartmentId)
        {
            try
            {
                Console.WriteLine($"=== GetApartmentResidents called for apartment: {apartmentId} ===");

                var residents = await _managerService.GetApartmentResidentsForManagerAssignmentAsync(apartmentId);

                Console.WriteLine($"Found {residents.Count} resident owners in this apartment");

                return Ok(ApiResponse<List<AvailableManagerDto>>.SuccessResponse(
                    residents,
                    residents.Count > 0
                        ? "Apartment residents retrieved successfully"
                        : "No resident owners found in this apartment"
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in GetApartmentResidents: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return BadRequest(ApiResponse<List<AvailableManagerDto>>.ErrorResponse(
                    $"Failed to load apartment residents: {ex.Message}"
                ));
            }
        }

        /// <summary>
        /// Assign a manager to an apartment.
        /// Supports BOTH internal (existing resident) and external (new user) flows.
        /// Route: POST /api/Manager/assign
        /// </summary>
        [HttpPost("assign")]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerRequestDto dto)
        {
            try
            {
                Console.WriteLine("=== AssignManager Called ===");
                Console.WriteLine($"ApartmentId: {dto.ApartmentId}");
                Console.WriteLine($"IsExternal: {dto.IsExternalManager}");
                Console.WriteLine($"UserId: {dto.UserId}");
                Console.WriteLine($"ExternalName: {dto.ExternalManagerName}");

                var assignedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _managerService.AssignManagerToApartmentAsync(dto, assignedByUserId);

                return Ok(ApiResponse<ManagerAssignmentDto>.SuccessResponse(
                    result,
                    "Manager assigned successfully"
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in AssignManager: {ex.Message}");
                return BadRequest(ApiResponse<ManagerAssignmentDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Remove manager from an apartment.
        /// Route: POST /api/Manager/remove
        /// </summary>
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveManager([FromBody] RemoveManagerRequestDto dto)
        {
            try
            {
                Console.WriteLine($"=== RemoveManager Called === ApartmentId={dto.ApartmentId}");

                var removedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _managerService.RemoveManagerFromApartmentAsync(dto, removedByUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    "Manager removed successfully"
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in RemoveManager: {ex.Message}");
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get all managers in the system.
        /// Route: GET /api/Manager/all
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



*/
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
        private readonly IManagerService _managerService;

        public ManagerApiController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        /// <summary>
        /// Get ResidentOwners from this apartment who can be assigned as manager
        /// </summary>
        [HttpGet("apartment-residents/{apartmentId}")]
        public async Task<IActionResult> GetApartmentResidents(Guid apartmentId)
        {
            try
            {
                var residents = await _managerService.GetApartmentResidentsForManagerAssignmentAsync(apartmentId);

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
        /// Remove manager from apartment
        /// </summary>
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveManager([FromBody] RemoveManagerRequestDto dto)
        {
            try
            {
                var removedByUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _managerService.RemoveManagerFromApartmentAsync(dto, removedByUserId);

                return Ok(ApiResponse<bool>.SuccessResponse(result, "Manager removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}


