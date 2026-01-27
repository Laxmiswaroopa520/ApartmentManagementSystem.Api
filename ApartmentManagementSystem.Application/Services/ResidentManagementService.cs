using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Application.Services
{
    public class ResidentManagementService : IResidentManagementService
    {
        private readonly IResidentManagementRepository ResidentManagementRepo;

        public ResidentManagementService(IResidentManagementRepository repository)
        {
            ResidentManagementRepo = repository;
        }

        public async Task<List<ResidentListDto>> GetAllResidentsAsync()
        {
            return await ResidentManagementRepo.GetAllResidentsAsync();
        }

        public async Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType)
        {
            return await ResidentManagementRepo.GetResidentsByTypeAsync(residentType);
        }

        public async Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId)
        {
            return await ResidentManagementRepo.GetResidentDetailAsync(userId);
        }

        public async Task<bool> DeactivateResidentAsync(Guid userId, Guid deactivatedBy)
        {
            await ResidentManagementRepo.SetResidentActiveStatusAsync(userId, false, deactivatedBy);
            return true;
        }

        public async Task<bool> ActivateResidentAsync(Guid userId, Guid activatedBy)
        {
            await ResidentManagementRepo.SetResidentActiveStatusAsync(userId, true, activatedBy);
            return true;
        }
    }
}
