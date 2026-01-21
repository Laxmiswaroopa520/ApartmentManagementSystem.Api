using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Application.Services
{
    public class ResidentManagementService : IResidentManagementService
    {
        private readonly IResidentManagementRepository _repository;

        public ResidentManagementService(IResidentManagementRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ResidentListDto>> GetAllResidentsAsync()
        {
            return await _repository.GetAllResidentsAsync();
        }

        public async Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType)
        {
            return await _repository.GetResidentsByTypeAsync(residentType);
        }

        public async Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId)
        {
            return await _repository.GetResidentDetailAsync(userId);
        }

        public async Task<bool> DeactivateResidentAsync(Guid userId, Guid deactivatedBy)
        {
            await _repository.SetResidentActiveStatusAsync(userId, false, deactivatedBy);
            return true;
        }

        public async Task<bool> ActivateResidentAsync(Guid userId, Guid activatedBy)
        {
            await _repository.SetResidentActiveStatusAsync(userId, true, activatedBy);
            return true;
        }
    }
}
