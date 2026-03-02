using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for handling resident management operations.
    /// 
    /// Handles:
    /// - Retrieving residents
    /// - Filtering residents by type
    /// - Fetching detailed resident information
    /// - Activating and deactivating resident accounts
    /// </summary>
    public class ResidentManagementService : IResidentManagementService
    {
        /// <summary>
        /// Repository responsible for resident management data access.
        /// </summary>
        private readonly IResidentManagementRepository ResidentManagementRepo;

        /// <summary>
        /// Constructor for injecting resident management repository dependency.
        /// </summary>
        /// <param name="repository">
        /// Repository that performs resident-related database operations.
        /// </param>
        public ResidentManagementService(IResidentManagementRepository repository)
        {
            ResidentManagementRepo = repository;
        }

        /// <summary>
        /// Retrieves all residents in the system.
        /// </summary>
        /// <returns>
        /// List of residents.
        /// </returns>
        public async Task<List<ResidentListDto>> GetAllResidentsAsync()
        {
            return await ResidentManagementRepo.GetAllResidentsAsync();
        }

        /// <summary>
        /// Retrieves residents filtered by type (e.g., Owner or Tenant).
        /// </summary>
        /// <param name="residentType">
        /// Type of resident to filter by.
        /// </param>
        /// <returns>
        /// List of residents matching the specified type.
        /// </returns>
        public async Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType)
        {
            return await ResidentManagementRepo.GetResidentsByTypeAsync(residentType);
        }

        /// <summary>
        /// Retrieves detailed information for a specific resident.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the resident.
        /// </param>
        /// <returns>
        /// Resident details if found; otherwise null.
        /// </returns>
        public async Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId)
        {
            return await ResidentManagementRepo.GetResidentDetailAsync(userId);
        }

        /// <summary>
        /// Deactivates a resident account.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the resident.
        /// </param>
        /// <param name="deactivatedBy">
        /// User ID of the person performing the deactivation.
        /// </param>
        /// <returns>
        /// True if operation succeeds.
        /// </returns>
        public async Task<bool> DeactivateResidentAsync(Guid userId, Guid deactivatedBy)
        {
            await ResidentManagementRepo
                .SetResidentActiveStatusAsync(userId, false, deactivatedBy);

            return true;
        }

        /// <summary>
        /// Activates a previously deactivated resident account.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the resident.
        /// </param>
        /// <param name="activatedBy">
        /// User ID of the person performing the activation.
        /// </param>
        /// <returns>
        /// True if operation succeeds.
        /// </returns>
        public async Task<bool> ActivateResidentAsync(Guid userId, Guid activatedBy)
        {
            await ResidentManagementRepo
                .SetResidentActiveStatusAsync(userId, true, activatedBy);

            return true;
        }
    }
}














/*using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
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
*/