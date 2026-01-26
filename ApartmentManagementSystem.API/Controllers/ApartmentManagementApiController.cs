    // API/Controllers/ApartmentManagementApiController.cs
    using ApartmentManagementSystem.Application.DTOs.Apartment;
    using ApartmentManagementSystem.Application.DTOs.Common;
    using ApartmentManagementSystem.Application.Interfaces.Services;
    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
using System.Text;
using System.Text.Json;

    namespace ApartmentManagementSystem.API.Controllers
    {
        [ApiController]
       // [Route("api/[controller]")]
    [Route("api/ApartmentManagement")]
    [Authorize(Roles = "SuperAdmin")]
        public class ApartmentManagementApiController : ControllerBase
        {
            private readonly IApartmentManagementService _apartmentService;

            public ApartmentManagementApiController(IApartmentManagementService apartmentService)
            {
                _apartmentService = apartmentService;
            }

        /// <summary>
        /// Create new apartment with floors and flats
        /// </summary>
       
        [HttpPost("create")]
         public async Task<IActionResult> CreateApartment([FromBody] CreateApartmentDto dto)
         {
             try
             {
                 var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                 var result = await _apartmentService.CreateApartmentAsync(dto, userId);

                 return Ok(ApiResponse<CreateApartmentResponseDto>.SuccessResponse(
                     result,
                     "Apartment created successfully with all floors and flats"
                 ));
             }
             catch (Exception ex)
             {
                 return BadRequest(ApiResponse<CreateApartmentResponseDto>.ErrorResponse(ex.Message));
             }
         }
     
        /// <summary>
        /// Get all apartments managed by SuperAdmin
        /// </summary>
        [HttpGet("all")]
            public async Task<IActionResult> GetAllApartments()
            {
                try
                {
                    var apartments = await _apartmentService.GetAllApartmentsAsync();
                    return Ok(ApiResponse<List<ApartmentListDto>>.SuccessResponse(
                        apartments,
                        "Apartments retrieved successfully"
                    ));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<List<ApartmentListDto>>.ErrorResponse(ex.Message));
                }
            }

            /// <summary>
            /// Get detailed apartment information
            /// </summary>
            [HttpGet("{apartmentId}")]
            public async Task<IActionResult> GetApartmentDetail(Guid apartmentId)
            {
                try
                {
                    var apartment = await _apartmentService.GetApartmentDetailAsync(apartmentId);

                    if (apartment == null)
                    {
                        return NotFound(ApiResponse<ApartmentDetailDto>.ErrorResponse("Apartment not found"));
                    }

                    return Ok(ApiResponse<ApartmentDetailDto>.SuccessResponse(
                        apartment,
                        "Apartment details retrieved successfully"
                    ));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<ApartmentDetailDto>.ErrorResponse(ex.Message));
                }
            }

            /// <summary>
            /// Get apartment visual diagram for 3D/2D rendering
            /// </summary>
            [HttpGet("{apartmentId}/diagram")]
            public async Task<IActionResult> GetApartmentDiagram(Guid apartmentId)
            {
                try
                {
                    var diagram = await _apartmentService.GetApartmentDiagramAsync(apartmentId);
                    return Ok(ApiResponse<ApartmentDiagramDto>.SuccessResponse(
                    diagram,
                    "Apartment diagram generated successfully"
                    ));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<ApartmentDiagramDto>.ErrorResponse(ex.Message));
                }
            }
            /// <summary>
            /// Assign manager to apartment
            /// </summary>
            [HttpPost("assign-manager")]
            public async Task<IActionResult> AssignManager([FromBody] AssignManagerDto dto)
            {
                try
                {
                    var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                    var result = await _apartmentService.AssignManagerAsync(dto, userId);

                    return Ok(ApiResponse<bool>.SuccessResponse(
                        result,
                        "Manager assigned successfully"
                    ));
                }
                catch (Exception ex)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
                }
            }
        }
    }