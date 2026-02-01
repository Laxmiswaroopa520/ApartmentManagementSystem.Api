/*

using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController]
    [Route("api/CommunityMembers")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class CommunityMembersApiController : ControllerBase
    {
        private readonly ICommunityMemberService _communityService;

        public CommunityMembersApiController(ICommunityMemberService communityService)
        {
            _communityService = communityService;
        }

        /// <summary>
        /// Get all community members (optionally filtered by apartmentId)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCommunityMembers([FromQuery] Guid? apartmentId = null)
        {
            try
            {
                var members = await _communityService.GetAllCommunityMembersAsync(apartmentId);
                return Ok(ApiResponse<List<CommunityMemberDto>>.SuccessResponse(
                    members,
                    "Community members retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CommunityMemberDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Get eligible resident owners from a specific apartment
        /// </summary>
        [HttpGet("eligible-residents/{apartmentId}")]
        public async Task<IActionResult> GetEligibleResidents(Guid apartmentId)
        {
            try
            {
                var residents = await _communityService.GetEligibleResidentsForApartmentAsync(apartmentId);
                return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                    residents,
                    "Eligible residents retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ResidentListDto>>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Assign community role to a resident owner
        /// </summary>
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignCommunityRole([FromBody] AssignCommunityRoleRequest request)
        {
            try
            {
                if (!request.ApartmentId.HasValue)
                {
                    return BadRequest(ApiResponse<CommunityMemberDto>.ErrorResponse("Apartment ID is required"));
                }

                var assignedBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _communityService.AssignCommunityRoleAsync(
                    request.UserId,
                    request.CommunityRole,
                    request.ApartmentId.Value,
                    assignedBy
                );

                return Ok(ApiResponse<CommunityMemberDto>.SuccessResponse(
                    result,
                    $"{request.CommunityRole} role assigned successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CommunityMemberDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Remove community role from a user
        /// </summary>
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveCommunityRole([FromBody] RemoveCommunityRoleRequest request)
        {
            try
            {
                await _communityService.RemoveCommunityRoleAsync(request.UserId);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Community role removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}

*/
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    [ApiController]
    [Route("api/CommunityMembers")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class CommunityMembersApiController : ControllerBase
    {
        private readonly ICommunityMemberService _communityService;

        public CommunityMembersApiController(ICommunityMemberService communityService)
        {
            _communityService = communityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommunityMembers([FromQuery] Guid? apartmentId = null)
        {
            try
            {
                var members = await _communityService.GetAllCommunityMembersAsync(apartmentId);
                return Ok(ApiResponse<List<CommunityMemberDto>>.SuccessResponse(
                    members,
                    "Community members retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CommunityMemberDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("eligible-residents/{apartmentId}")]
        public async Task<IActionResult> GetEligibleResidents(Guid apartmentId)
        {
            try
            {
                var residents = await _communityService.GetEligibleResidentsForApartmentAsync(apartmentId);
                return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                    residents,
                    "Eligible residents retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ResidentListDto>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignCommunityRole([FromBody] AssignCommunityRoleRequestDto request)
        {
            try
            {
                if (!request.ApartmentId.HasValue)
                {
                    return BadRequest(ApiResponse<CommunityMemberDto>.ErrorResponse("Apartment ID is required"));
                }

                var assignedBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _communityService.AssignCommunityRoleAsync(
                    request.UserId,
                    request.CommunityRole,
                    request.ApartmentId.Value,
                    assignedBy
                );

                return Ok(ApiResponse<CommunityMemberDto>.SuccessResponse(
                    result,
                    $"{request.CommunityRole} role assigned successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CommunityMemberDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveCommunityRole([FromBody] RemoveCommunityRoleRequestDto request)
        {
            try
            {
                await _communityService.RemoveCommunityRoleAsync(request.UserId);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Community role removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}












