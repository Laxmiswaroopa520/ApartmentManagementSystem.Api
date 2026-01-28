using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
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
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class AdminResidentApiController : ControllerBase
    {
        private readonly IAdminResidentService _adminResidentService;

        public AdminResidentApiController(IAdminResidentService adminResidentService)
        {
            _adminResidentService = adminResidentService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingResidents()
        {
            try
            {
                var residents = await _adminResidentService.GetPendingResidentsAsync();
                return Ok(ApiResponse<List<PendingResidentDto>>.SuccessResponse(residents));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<PendingResidentDto>>.ErrorResponse(ex.Message));
            }
        }

        // ⭐ NEW: Get apartments for current user
        [HttpGet("apartments")]
        public async Task<IActionResult> GetApartments()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

                var apartments = await _adminResidentService.GetApartmentsForUserAsync(userId, role);
                return Ok(ApiResponse<List<ApartmentDropdownDto>>.SuccessResponse(apartments));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ApartmentDropdownDto>>.ErrorResponse(ex.Message));
            }
        }

        // ⭐ NEW: Get floors by apartment
        [HttpGet("apartments/{apartmentId}/floors")]
        public async Task<IActionResult> GetFloorsByApartment(Guid apartmentId)
        {
            try
            {
                var floors = await _adminResidentService.GetFloorsByApartmentAsync(apartmentId);
                return Ok(ApiResponse<List<FloorDto>>.SuccessResponse(floors));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<FloorDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("floors/{floorId}/flats")]
        public async Task<IActionResult> GetVacantFlatsByFloor(Guid floorId)
        {
            try
            {
                var flats = await _adminResidentService.GetVacantFlatsByFloorAsync(floorId);
                return Ok(ApiResponse<List<FlatDto>>.SuccessResponse(flats));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<FlatDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("assign-flat")]
        public async Task<IActionResult> AssignFlat([FromBody] AssignFlatDto dto)
        {
            try
            {
                var result = await _adminResidentService.AssignFlatToResidentAsync(dto);
                return Ok(ApiResponse<AssignFlatResponseDto>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<AssignFlatResponseDto>.ErrorResponse(ex.Message));
            }
        }
    }
}









/*
using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin,Manager")]
public class AdminResidentApiController : ControllerBase
{
    private readonly IAdminResidentService AdminResidentService;

    public AdminResidentApiController(IAdminResidentService adminResidentService)
    {
        AdminResidentService = adminResidentService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingResidents()
    {
        try
        {
            var residents = await AdminResidentService.GetPendingResidentsAsync();
            return Ok(ApiResponse<List<PendingResidentDto>>.SuccessResponse(residents));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<PendingResidentDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("assign-flat")]
    public async Task<IActionResult> AssignFlat([FromBody] AssignFlatDto dto)
    {
        try
        {
            var result = await AdminResidentService.AssignFlatToResidentAsync(dto);
            return Ok(ApiResponse<AssignFlatResponseDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AssignFlatResponseDto>.ErrorResponse(ex.Message));
        }
    }
    [HttpGet("floors")]     // GET /api/AdminResidentApi/floors
    public async Task<IActionResult> GetFloors()
    {
        try
        {
            var floors = await AdminResidentService.GetAllFloorsAsync();
            return Ok(ApiResponse<List<FloorDto>>.SuccessResponse(floors));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<FloorDto>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("floors/{floorId}/flats")] // GET /api/AdminResidentApi/floors/{floorId}/flats
    public async Task<IActionResult> GetVacantFlatsByFloor(Guid floorId)
    {
        try
        {
            var flats = await AdminResidentService.GetVacantFlatsByFloorAsync(floorId);
            return Ok(ApiResponse<List<FlatDto>>.SuccessResponse(flats));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<FlatDto>>.ErrorResponse(ex.Message));
        }
    }

}
*/