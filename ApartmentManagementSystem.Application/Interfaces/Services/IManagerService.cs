using ApartmentManagementSystem.Application.DTOs.Manager;
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IManagerService
    {
       //Get ResidentOwners from THIS apartment who can be made managers
        Task<List<AvailableManagerDto>> GetApartmentResidentsForManagerAssignmentAsync(Guid apartmentId);
        /// Assign manager to apartment - supports both resident and external
        Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(AssignManagerRequestDto dto, Guid assignedBy);

        /// Remove manager from apartment
        Task<bool> RemoveManagerFromApartmentAsync(RemoveManagerRequestDto dto, Guid removedBy);

        /// Get all current managers
        Task<List<ManagerListDto>> GetAllManagersAsync();

        /// Get manager details by user ID
        Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId);
    }
}