using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Onboarding;

using ApartmentManagementSystem.Application.DTOs.Apartment;
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IAdminResidentService
    {
        Task<List<PendingResidentDto>> GetPendingResidentsAsync();
        Task<AssignFlatResponseDto> AssignFlatToResidentAsync(AssignFlatDto dto);

        // Multi-apartment support
        Task<List<ApartmentDropdownDto>> GetApartmentsForUserAsync(Guid userId, string role);
        Task<List<FloorDto>> GetFloorsByApartmentAsync(Guid apartmentId);
        Task<List<FlatDto>> GetVacantFlatsByFloorAsync(Guid floorId);

        // Task<List<FloorDto>> GetAllFloorsAsync();
    }
}
