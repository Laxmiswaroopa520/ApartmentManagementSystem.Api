using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
        [ApiController]
        [Route("api/[controller]")]
    // [Authorize(Roles = "SuperAdmin,Manager")]
    [Authorize(Roles = SystemRoles.SuperAdmin + "," + SystemRoles.Manager)]
    public class AdminResidentController : ControllerBase
        {
            /// <summary>
            /// Service responsible for handling resident-related business logic.
            /// </summary>
            private readonly IAdminResidentService AdminResidentService;

            /// <summary>
            /// Constructor for injecting IAdminResidentService dependency.
            /// </summary>
            /// <param name="adminResidentService">
            /// Service implementation that contains business logic 
            /// for resident and flat management.
            /// </param>
            public AdminResidentController(IAdminResidentService adminResidentService)
            {
                AdminResidentService = adminResidentService;
            }

            /// <summary>
            /// Retrieves all residents whose onboarding status is pending.
            /// Accessible only by SuperAdmin and Manager.
            /// </summary>
            /// <returns>
            /// List of pending residents wrapped inside ApiResponse.
            /// </returns>
            [HttpGet("pending")]
            public async Task<IActionResult> GetPendingResidents()
            {
                try
                {
                    var residents = await AdminResidentService.GetPendingResidentsAsync();

                    return Ok(ApiResponse<List<PendingResidentDto>>
                              .SuccessResponse(residents));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<List<PendingResidentDto>>
                                      .ErrorResponse(ex.Message));
                }
            }

            /// <summary>
            /// Retrieves apartments accessible to the currently logged-in user.
            /// 
            /// - SuperAdmin → May get all apartments
            /// - Manager → Gets only assigned apartments
            /// </summary>
            /// <returns>
            /// List of apartments in dropdown format.
            /// </returns>
            [HttpGet("apartments")]
            public async Task<IActionResult> GetApartments()
            {
                try
                {
                    // Extract current logged-in user ID from JWT claims
                    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                    // Extract user role from JWT claims
                    var role = User.FindFirstValue(ClaimTypes.Role) ?? "";

                    var apartments = await AdminResidentService
                        .GetApartmentsForUserAsync(userId, role);

                    return Ok(ApiResponse<List<ApartmentDropdownDto>>
                              .SuccessResponse(apartments));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<List<ApartmentDropdownDto>>
                                      .ErrorResponse(ex.Message));
                }
            }

            /// <summary>
            /// Retrieves all floors for a specific apartment.
            /// </summary>
            /// <param name="apartmentId">Unique identifier of the apartment.</param>
            /// <returns>
            /// List of floors belonging to the specified apartment.
            /// </returns>
            [HttpGet("apartments/{apartmentId}/floors")]
            public async Task<IActionResult> GetFloorsByApartment(Guid apartmentId)
            {
                try
                {
                    var floors = await AdminResidentService
                        .GetFloorsByApartmentAsync(apartmentId);

                    return Ok(ApiResponse<List<FloorDto>>
                              .SuccessResponse(floors));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<List<FloorDto>>
                                      .ErrorResponse(ex.Message));
                }
            }

            /// <summary>
            /// Retrieves vacant flats for a given floor.
            /// Only flats that are not assigned to any resident are returned.
            /// </summary>
            /// <param name="floorId">Unique identifier of the floor.</param>
            /// <returns>
            /// List of vacant flats.
            /// </returns>
            [HttpGet("floors/{floorId}/flats")]
            public async Task<IActionResult> GetVacantFlatsByFloor(Guid floorId)
            {
                try
                {
                    var flats = await AdminResidentService
                        .GetVacantFlatsByFloorAsync(floorId);

                    return Ok(ApiResponse<List<FlatDto>>
                              .SuccessResponse(flats));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<List<FlatDto>>
                                      .ErrorResponse(ex.Message));
                }
            }

            /// <summary>
            /// Assigns a flat to a resident.
            /// This operation typically updates:
            /// - Resident status
            /// - Flat occupancy status
            /// </summary>
            /// <param name="dto">
            /// Contains ResidentId, ApartmentId, FloorId, and FlatId details.
            /// </param>
            /// <returns>
            /// Assignment confirmation response.
            /// </returns>
            [HttpPost("assign-flat")]
            public async Task<IActionResult> AssignFlat([FromBody] AssignFlatDto dto)
            {
                try
                {
                    var result = await AdminResidentService
                        .AssignFlatToResidentAsync(dto);

                    return Ok(ApiResponse<AssignFlatResponseDto>
                              .SuccessResponse(result));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<AssignFlatResponseDto>
                                      .ErrorResponse(ex.Message));
                }
            }
        }
    }





















































