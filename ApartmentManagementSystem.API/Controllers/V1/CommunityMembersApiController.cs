using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    /// <summary>
    /// Handles community member management operations such as:
    /// - Viewing community members
    /// - Fetching eligible residents
    /// - Assigning community roles
    /// - Removing community roles
    /// 
    /// Accessible only by SuperAdmin and Manager roles.
    /// </summary>
    [ApiController]
    [Route("api/CommunityMembers")]
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class CommunityMembersApiController : ControllerBase
    {
        /// <summary>
        /// Service responsible for community member business logic.
        /// </summary>
        private readonly ICommunityMemberService CommunityService;

        /// <summary>
        /// Constructor for injecting community member service.
        /// </summary>
        /// <param name="communityService">
        /// Service that handles community member and role operations.
        /// </param>
        public CommunityMembersApiController(ICommunityMemberService communityService)
        {
            CommunityService = communityService;
        }

        /// <summary>
        /// Retrieves all community members.
        /// 
        /// If apartmentId is provided, results are filtered by apartment.
        /// </summary>
        /// <param name="apartmentId">
        /// Optional apartment identifier to filter members.
        /// </param>
        /// <returns>
        /// List of community members wrapped inside ApiResponse.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCommunityMembers([FromQuery] Guid? apartmentId = null)
        {
            try
            {
                var members = await CommunityService
                    .GetAllCommunityMembersAsync(apartmentId);

                return Ok(ApiResponse<List<CommunityMemberDto>>.SuccessResponse(
                    members,
                    CommunityMessages.CommunityMembersRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<CommunityMemberDto>>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Retrieves residents eligible for community role assignment
        /// within a specific apartment.
        /// </summary>
        /// <param name="apartmentId">
        /// Unique identifier of the apartment.
        /// </param>
        /// <returns>
        /// List of eligible residents.
        /// </returns>
        [HttpGet("eligible-residents/{apartmentId}")]
        public async Task<IActionResult> GetEligibleResidents(Guid apartmentId)
        {
            try
            {
                var residents = await CommunityService
                    .GetEligibleResidentsForApartmentAsync(apartmentId);

                return Ok(ApiResponse<List<ResidentListDto>>.SuccessResponse(
                    residents,
                    CommunityMessages.EligibleResidentsRetrieved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ResidentListDto>>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Assigns a community role (e.g., President, Treasurer, etc.)
        /// to a specified user within an apartment.
        /// </summary>
        /// <param name="request">
        /// Contains UserId, CommunityRole, and ApartmentId details.
        /// </param>
        /// <returns>
        /// Assigned community member details.
        /// </returns>
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignCommunityRole(
            [FromBody] AssignCommunityRoleRequestDto request)
        {
            try
            {
                if (request == null || !request.ApartmentId.HasValue)
                {
                    return BadRequest(ApiResponse<CommunityMemberDto>.ErrorResponse(
                        CommunityMessages.ApartmentIdRequired
                    ));
                }

                // Get currently logged-in user ID from JWT claims
                var assignedBy = Guid.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)!
                );

                var result = await CommunityService.AssignCommunityRoleAsync(
                    request.UserId,
                    request.CommunityRole,
                    request.ApartmentId.Value,
                    assignedBy
                );

                return Ok(ApiResponse<CommunityMemberDto>.SuccessResponse(
                    result,
                    $"{request.CommunityRole} {CommunityMessages.RoleAssigned}"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<CommunityMemberDto>
                    .ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Removes a community role from a specified user.
        /// </summary>
        /// <param name="request">
        /// Contains UserId of the community member.
        /// </param>
        /// <returns>
        /// Boolean response indicating successful removal.
        /// </returns>
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveCommunityRole(
            [FromBody] RemoveCommunityRoleRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ApiResponse<bool>
                        .ErrorResponse("Invalid request."));
                }

                await CommunityService.RemoveCommunityRoleAsync(request.UserId);

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    CommunityMessages.RoleRemoved
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>
                    .ErrorResponse(ex.Message));
            }
        }
    }
}




















































