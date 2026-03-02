using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController]
    [Route("api/ApartmentManagement")]
    [Authorize(Roles = "SuperAdmin")]
    public class ApartmentManagementApiController : ControllerBase
    {
        private readonly IApartmentManagementService ApartmentService;

        public ApartmentManagementApiController(IApartmentManagementService apartmentService)
        {
            ApartmentService = apartmentService;
        }

        // Create new apartment with floors and flats
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

        // Get all apartments
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

        // Get apartment details
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

        // Get apartment diagram
        [HttpGet("{apartmentId}/diagram")]
        public async Task<IActionResult> GetApartmentDiagram(Guid apartmentId)
        {
            try
            {
                var diagram = await ApartmentService.GetApartmentDiagramAsync(apartmentId);

                return Ok(ApiResponse<ApartmentDiagramDto>.SuccessResponse(
                    diagram,
                    ResponseMessages.ApartmentDiagramGenerated
                ));                     //calling response messages from constants folder..
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ApartmentDiagramDto>.ErrorResponse(ex.Message));
            }
        }

        // Assign manager
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
    }
}




























