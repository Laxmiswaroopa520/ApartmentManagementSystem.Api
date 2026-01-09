using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    using ApartmentManagementSystem.Application.DTOs.Onboarding;
    using global::ApartmentManagementSystem.Application.DTOs.Onboarding;

   // namespace ApartmentManagementSystem.Application.Interfaces.Services;

    public interface IOnboardingService
    {
        Task<CreateInviteResponseDto> CreateInviteAsync(CreateUserInviteDto dto, Guid createdBy);
        Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
        Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(CompleteRegistrationDto dto);
    }
}