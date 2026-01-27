using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IResidentManagementRepository
    {
        Task<List<ResidentListDto>> GetAllResidentsAsync();
        Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType);
        Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId);

        Task SetResidentActiveStatusAsync(Guid userId, bool isActive, Guid updatedBy);
    }
}
