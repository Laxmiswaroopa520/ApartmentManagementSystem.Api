namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    using ApartmentManagementSystem.Application.DTOs.Auth;
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
       //used for checking inactive users..
        Task<bool> IsUserActiveAsync(Guid userId);
    }
}