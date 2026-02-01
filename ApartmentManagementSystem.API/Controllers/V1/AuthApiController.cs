using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.DTOs.Common;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ApartmentManagementSystem.API.Controllers.V1;

[ApiController]
[Route("api/[controller]")]
//[ApiVersion("1.0")]  
//[Route("api/v{version:apiVersion}/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly IAuthService AuthService;

    public AuthApiController(IAuthService authService)
    {
        AuthService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var result = await AuthService.LoginAsync(request);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)          //validate login checks status and return structured error
        {
            var message = ex.Message;

            if (message.Contains("inactive"))
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(
                    message,
                    "ACCOUNT_INACTIVE"
                ));
            }

            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(message));
        }

       // catch (UnauthorizedAccessException ex)
      //  {
       //     return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
      //  }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /* [HttpGet("users/{userId}/is-active")]
     [Authorize]
     public async Task<IActionResult> IsUserActive(Guid userId)
     {
         var user = await Users.GetByIdAsync(userId);
         return Ok(user != null && user.IsActive);
     }*/
    [HttpGet("users/{userId}/is-active")]
    [Authorize]
    public async Task<IActionResult> IsUserActive(Guid userId)
    {
        var isActive = await AuthService.IsUserActiveAsync(userId);
        return Ok(isActive);
    }



}
