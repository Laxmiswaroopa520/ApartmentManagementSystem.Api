
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    using ApartmentManagementSystem.Application.DTOs.Onboarding;

   // namespace ApartmentManagementSystem.Application.Interfaces.Services;

    public interface IOnboardingService
    {
        Task<CreateInviteResponseDto> CreateInviteAsync(CreateUserInviteDto dto, Guid createdBy);
        Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpDto dto);
        Task<CompleteRegistrationResponseDto> CompleteRegistrationAsync(CompleteRegistrationDto dto);
      //  Task<List<FloorDto>> GetFloorsAsync();
      //  Task<List<FlatDto>> GetAvailableFlatsByFloorAsync(Guid floorId);

    }
}