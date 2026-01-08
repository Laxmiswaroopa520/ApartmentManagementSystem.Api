using ApartmentManagementSystem.Application.DTOs.Onboarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IOnboardingService
    {
        Task CreateInviteAsync(CreateInviteRequest request);
        Task VerifyOtpAsync(VerifyOtpRequest request);
        Task CompleteRegistrationAsync(CompleteRegistrationRequest request);
    }
}