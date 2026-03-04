using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    /// <summary>
    /// REST API controller for apartment management operations.
    /// Handles the full lifecycle of an apartment building: creation, retrieval,
    /// diagram generation, manager assignment, and deletion.
    /// All endpoints are restricted to the SuperAdmin role.
    /// </summary>
    [ApiController]
    [Route("api/ApartmentManagement")]
    [Authorize(Roles = "SuperAdmin")]
    public class ApartmentManagementApiController : ControllerBase
    {
        private readonly IApartmentManagementService ApartmentService;

        /// <summary>
        /// Injects the apartment management service that encapsulates
        /// all apartment-related business logic and database operations.
        /// </summary>
        public ApartmentManagementApiController(IApartmentManagementService apartmentService)
        {
            ApartmentService = apartmentService;
        }

        /// <summary>
        /// Creates a new apartment building along with its floors and flats
        /// based on the provided configuration (total floors, flats per floor).
        /// The authenticated SuperAdmin is recorded as the creator.
        /// </summary>
        /// <param name="dto">
        /// Apartment creation data including name, address, total floors,
        /// and flats per floor configuration.
        /// </param>
        /// <returns>
        /// The creation response including the new apartment's ID,
        /// total floors created, and total flats generated.
        /// </returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateApartment([FromBody] CreateApartmentDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await ApartmentService.CreateApartmentAsync(dto, userId);

                return Ok(ApiResponse<CreateApartmentResponseDto>.SuccessResponse(
                    result,
                    ResponseMessages.ApartmentCreated
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CreateApartmentResponseDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves a summary list of all apartment buildings in the system.
        /// Each entry includes the apartment name, address, status, floor count,
        /// flat count, occupied flat count, and occupancy percentage.
        /// Used to populate the Manage Apartments grid view.
        /// </summary>
        /// <returns>List of apartment summary DTOs.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllApartments()
        {
            try
            {
                var apartments = await ApartmentService.GetAllApartmentsAsync();

                return Ok(ApiResponse<List<ApartmentListDto>>.SuccessResponse(
                    apartments,
                    ResponseMessages.ApartmentsRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ApartmentListDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the full detail view of a specific apartment including
        /// its floors, flats, assigned manager information, and community member list.
        /// Returns 404 if the apartment does not exist.
        /// </summary>
        /// <param name="apartmentId">The GUID of the apartment to retrieve details for.</param>
        /// <returns>Full apartment detail DTO, or 404 if not found.</returns>
        [HttpGet("{apartmentId}")]
        public async Task<IActionResult> GetApartmentDetail(Guid apartmentId)
        {
            try
            {
                var apartment = await ApartmentService.GetApartmentDetailAsync(apartmentId);

                if (apartment == null)
                {
                    return NotFound(ApiResponse<ApartmentDetailDto>.ErrorResponse(
                        ResponseMessages.ApartmentNotFound
                    ));
                }

                return Ok(ApiResponse<ApartmentDetailDto>.SuccessResponse(
                    apartment,
                    ResponseMessages.ApartmentDetailsRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ApartmentDetailDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Generates and returns the 3D/2D visual diagram data for a specific apartment.
        /// The diagram includes a structured floor-by-floor breakdown with flat occupancy status
        /// used by the Visualize view to render the interactive building diagram.
        /// </summary>
        /// <param name="apartmentId">The GUID of the apartment to generate the diagram for.</param>
        /// <returns>Apartment diagram DTO containing floor and flat layout data.</returns>
        [HttpGet("{apartmentId}/diagram")]
        public async Task<IActionResult> GetApartmentDiagram(Guid apartmentId)
        {
            try
            {
                var diagram = await ApartmentService.GetApartmentDiagramAsync(apartmentId);

                return Ok(ApiResponse<ApartmentDiagramDto>.SuccessResponse(
                    diagram,
                    ResponseMessages.ApartmentDiagramGenerated
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ApartmentDiagramDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Assigns a manager to an apartment.
        /// Supports assigning both existing system users and external managers.
        /// Only one manager can be active per apartment at a time;
        /// assigning a new manager will replace the existing one.
        /// The authenticated SuperAdmin is recorded as the user performing the assignment.
        /// </summary>
        /// <param name="dto">
        /// Manager assignment data including the apartment ID, user ID,
        /// and whether the manager is an external user.
        /// </param>
        /// <returns>True if the assignment was successful.</returns>
        [HttpPost("assign-manager")]
        public async Task<IActionResult> AssignManager([FromBody] AssignManagerDto dto)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await ApartmentService.AssignManagerAsync(dto, userId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    ResponseMessages.ManagerAssigned
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Permanently deletes an apartment and all associated data including
        /// floors, flats, manager assignments, and community member records.
        /// This action is irreversible. Apartments with occupied flats
        /// should be validated on the client side before calling this endpoint.
        /// The authenticated SuperAdmin is recorded as the user performing the deletion.
        /// </summary>
        /// <param name="apartmentId">The GUID of the apartment to permanently delete.</param>
        /// <returns>True if deletion was successful.</returns>
        [HttpDelete("{apartmentId}")]
        public async Task<IActionResult> DeleteApartment(Guid apartmentId)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await ApartmentService.DeleteApartmentAsync(apartmentId, userId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    "Apartment deleted successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}