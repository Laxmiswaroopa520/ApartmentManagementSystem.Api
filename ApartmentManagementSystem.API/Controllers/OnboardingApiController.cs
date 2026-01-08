using Microsoft.AspNetCore.Mvc;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
namespace ApartmentManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/onboarding")]
    public class OnboardingApiController : ControllerBase
    {
        private readonly IOnboardingService Onboardingservice;

        public OnboardingApiController(IOnboardingService service)
        {
            Onboardingservice = service;
        }

        [HttpPost("invite")]
        public async Task<IActionResult> Invite(CreateInviteRequest request)
        {
            await Onboardingservice.CreateInviteAsync(request);
            return Ok();
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest request)
        {
            await Onboardingservice.VerifyOtpAsync(request);
            return Ok();
        }

        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration(CompleteRegistrationRequest request)
        {
            await Onboardingservice.CompleteRegistrationAsync(request);
            return Ok();
        }
    }
}