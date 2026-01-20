using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [HttpGet("floors")]     // GET /api/AdminResidentApi/floors
    public async Task<IActionResult> GetFloors()
    {
        try
        {
            var floors = await _adminResidentService.GetAllFloorsAsync();
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
            var flats = await _adminResidentService.GetVacantFlatsByFloorAsync(floorId);
            return Ok(ApiResponse<List<FlatDto>>.SuccessResponse(flats));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<FlatDto>>.ErrorResponse(ex.Message));
        }
    }

}