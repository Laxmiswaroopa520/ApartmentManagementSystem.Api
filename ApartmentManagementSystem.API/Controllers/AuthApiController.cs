using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

using ApartmentManagementSystem.Application.DTOs.Auth;
using ApartmentManagementSystem.Application.DTOs.Common;


namespace ApartmentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
        }
    }
}
