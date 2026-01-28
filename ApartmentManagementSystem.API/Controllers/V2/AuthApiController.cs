//Enhanced with refresh tokens
using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.DTOs.V2.Auth;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Interfaces.Services.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartmentManagementSystem.API.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]                              // ← V2
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthServiceV2 _authService;

        public AuthApiController(IAuthServiceV2 authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// V2: Enhanced login with refresh token support
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginWithRefreshTokenAsync(request);
                return Ok(ApiResponse<LoginResponseV2Dto>.SuccessResponse(
                    result,
                    "Login successful"
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = ex.Message;
                if (message.Contains("inactive"))
                {
                    return Unauthorized(ApiResponse<LoginResponseV2Dto>.ErrorResponse(
                        message,
                        "ACCOUNT_INACTIVE"
                    ));
                }
                return Unauthorized(ApiResponse<LoginResponseV2Dto>.ErrorResponse(message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LoginResponseV2Dto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// V2 NEW: Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request);
                return Ok(ApiResponse<RefreshTokenResponseDto>.SuccessResponse(
                    result,
                    "Token refreshed successfully"
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<RefreshTokenResponseDto>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<RefreshTokenResponseDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// V2 NEW: Revoke refresh token (logout from all devices)
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken()
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var result = await _authService.RevokeRefreshTokenAsync(userId);
                return Ok(ApiResponse<bool>.SuccessResponse(
                    result,
                    "Token revoked successfully"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("users/{userId}/is-active")]
        [Authorize]
        public async Task<IActionResult> IsUserActive(Guid userId)
        {
            var isActive = await _authService.IsUserActiveAsync(userId);
            return Ok(isActive);
        }
    }
}