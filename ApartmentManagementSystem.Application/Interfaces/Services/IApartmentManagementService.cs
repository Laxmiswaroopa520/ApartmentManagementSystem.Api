using ApartmentManagementSystem.Application.DTOs.Apartment;
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IApartmentManagementService
    {
        // Apartment CRUD
        Task<CreateApartmentResponseDto> CreateApartmentAsync(CreateApartmentDto dto, Guid createdBy);
        Task<List<ApartmentListDto>> GetAllApartmentsAsync();
        Task<ApartmentDetailDto?> GetApartmentDetailAsync(Guid apartmentId);
        Task<bool> UpdateApartmentAsync(Guid apartmentId, UpdateApartmentDto dto, Guid updatedBy);
        Task<bool> DeactivateApartmentAsync(Guid apartmentId, Guid deactivatedBy);

        // Visualization
        Task<ApartmentDiagramDto> GetApartmentDiagramAsync(Guid apartmentId);

        // Manager Assignment
        Task<bool> AssignManagerAsync(AssignManagerDto dto, Guid assignedBy);
        Task<bool> RemoveManagerAsync(Guid apartmentId, Guid userId, Guid removedBy);
    }
}