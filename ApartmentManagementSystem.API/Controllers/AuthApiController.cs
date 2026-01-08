using ApartmentManagementSystem.API.DTOs.Auth;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
namespace ApartmentManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService AuthService;

        public AuthApiController(IAuthService authService)
        {
            AuthService = authService;
        }

        /*  [HttpPost("login")]
          public async Task<IActionResult> Login(LoginRequestDto dto)
          {
              var result = await AuthService.LoginAsync(dto.Email, dto.Password);
              return Ok(result);
          }
        */

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(
            [FromBody] LoginRequestDto dto)
        {
            var result = await AuthService.LoginAsync(dto.Email, dto.Password);
            return Ok(result);
        }

        /*  [HttpPost]
          public async Task<IActionResult> Login(LoginViewDto model)
          {
              var result = await _authApiClient.LoginAsync(model);

              if (result == null)
              {
                  ModelState.AddModelError("", "Invalid login");
                  return View(model);
              }

              HttpContext.Session.SetString("JWT", result.Token);
              return RedirectToAction("Index", "Dashboard");
          }
        */
    }
}