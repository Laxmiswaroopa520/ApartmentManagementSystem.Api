using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.API.Controllers.V1
{
    /// <summary>
    /// Handles authentication-related operations such as:
    /// - User login
    /// - Checking user active status
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        /// <summary>
        /// Service responsible for authentication business logic.
        /// </summary>
        private readonly IAuthService AuthService;

        /// <summary>
        /// Constructor for injecting authentication service.
        /// </summary>
        /// <param name="authService">
        /// Service that handles login validation and user status checks.
        /// </param>
        public AuthApiController(IAuthService authService)
        {
            AuthService = authService;
        }

        /// <summary>
        /// Authenticates a user using username and password.
        /// 
        /// Returns JWT token and user details if credentials are valid.
        /// </summary>
        /// <param name="request">
        /// Login request containing username and password.
        /// </param>
        /// <returns>
        /// Success response with LoginResponseDto if authentication succeeds.
        /// 
        /// Possible responses:
        /// - 400 BadRequest → Missing username/password
        /// - 401 Unauthorized → Invalid credentials or inactive account
        /// - 200 OK → Successful login
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // Basic validation for required fields
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse(
                    AuthMessages.UsernamePasswordRequired
                ));
            }

            try
            {
                // Call authentication service
                var result = await AuthService.LoginAsync(request);

                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(
                    result,
                    AuthMessages.LoginSuccessful
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                var message = ex.Message;

                // Special handling for inactive accounts
                if (message.Contains("inactive", StringComparison.OrdinalIgnoreCase))
                {
                    return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(
                        message,
                        AuthMessages.AccountInactiveCode // Calling message code from constants
                    ));
                }

                return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(message));
            }
            catch (Exception ex)
            {
                // Generic exception handling
                return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Checks whether a specific user account is active.
        /// 
        /// This endpoint requires authentication.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the user.
        /// </param>
        /// <returns>
        /// Boolean value indicating whether the user is active.
        /// </returns>
        [HttpGet("users/{userId}/is-active")]
        [Authorize]
        public async Task<IActionResult> IsUserActive(Guid userId)
        {
            var isActive = await AuthService.IsUserActiveAsync(userId);

            return Ok(isActive);
        }
    }
}






















































