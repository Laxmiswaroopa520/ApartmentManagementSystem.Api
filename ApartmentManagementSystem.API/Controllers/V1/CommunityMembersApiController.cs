
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
        private readonly ICommunityMemberService CommunityService;

        public CommunityMembersApiController(ICommunityMemberService communityService)
        {
            CommunityService = communityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommunityMembers([FromQuery] Guid? apartmentId = null)
        {
            try
            {
                var members = await CommunityService.GetAllCommunityMembersAsync(apartmentId);
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
                var residents = await CommunityService.GetEligibleResidentsForApartmentAsync(apartmentId);
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

                var result = await CommunityService.AssignCommunityRoleAsync(
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
                await CommunityService.RemoveCommunityRoleAsync(request.UserId);
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Community role removed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
    }
}












