using System;

public interface IOnboardingService
{
    Task CreateInviteAsync(CreateInviteRequest request);
    Task VerifyOtpAsync(VerifyOtpRequest request);
    Task CompleteRegistrationAsync(CompleteRegistrationRequest request);
}
