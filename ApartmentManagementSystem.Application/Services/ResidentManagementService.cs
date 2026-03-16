using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for resident management operations.
    ///
    /// Handles:
    /// - Listing all residents or filtering by type (Owner/Tenant)
    /// - Retrieving detailed resident information
    /// - Activating and deactivating resident accounts
    /// </summary>
    public class ResidentManagementService : IResidentManagementService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>
        /// Initialises ResidentManagementService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        public ResidentManagementService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        /// <summary>
        /// Retrieves all residents (Owners and Tenants) in the system.
        /// Results are ordered by registration date descending.
        /// </summary>
        /// <returns>List of resident list DTOs.</returns>
        public async Task<List<ResidentListDto>> GetAllResidentsAsync()
            => await UoW.Residents.GetAllResidentsAsync();

        /// <summary>
        /// Retrieves residents filtered by type.
        /// </summary>
        /// <param name="residentType">Type of resident — "Owner" or "Tenant".</param>
        /// <returns>List of resident list DTOs matching the specified type.</returns>
        public async Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType)
            => await UoW.Residents.GetResidentsByTypeAsync(residentType);

        /// <summary>
        /// Retrieves full details for a specific resident including roles,
        /// flat assignment, and outstanding bill placeholder.
        /// </summary>
        /// <param name="userId">Unique identifier of the resident.</param>
        /// <returns>Resident detail DTO or null if not found.</returns>
        public async Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId)
            => await UoW.Residents.GetResidentDetailAsync(userId);

        /// <summary>
        /// Deactivates a resident account.
        ///
        /// Sets IsActive to false in memory and commits in one SaveChanges.
        /// The resident can no longer log in after deactivation.
        /// </summary>
        /// <param name="userId">Unique identifier of the resident to deactivate.</param>
        /// <param name="deactivatedBy">UserId of the admin performing the deactivation.</param>
        /// <returns>True on success.</returns>
        public async Task<bool> DeactivateResidentAsync(Guid userId, Guid deactivatedBy)
        {
            await UoW.Residents.SetResidentActiveStatusAsync(userId, false, deactivatedBy);
            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Re-activates a previously deactivated resident account.
        ///
        /// Sets IsActive to true in memory and commits in one SaveChanges.
        /// </summary>
        /// <param name="userId">Unique identifier of the resident to activate.</param>
        /// <param name="activatedBy">UserId of the admin performing the activation.</param>
        /// <returns>True on success.</returns>
        public async Task<bool> ActivateResidentAsync(Guid userId, Guid activatedBy)
        {
            await UoW.Residents.SetResidentActiveStatusAsync(userId, true, activatedBy);
            await UoW.SaveChangesAsync();
            return true;
        }
    }
}





















/*
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Application.Services
{
    public class ResidentManagementService : IResidentManagementService
    {
        private readonly IUnitOfWork UoW;

        public ResidentManagementService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public async Task<List<ResidentListDto>> GetAllResidentsAsync()
            => await UoW.Residents.GetAllResidentsAsync();

        public async Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType)
            => await UoW.Residents.GetResidentsByTypeAsync(residentType);

        public async Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId)
            => await UoW.Residents.GetResidentDetailAsync(userId);

        public async Task<bool> DeactivateResidentAsync(Guid userId, Guid deactivatedBy)
        {
            // Mutates in memory
            await UoW.Residents.SetResidentActiveStatusAsync(userId, false, deactivatedBy);

            // ONE SaveChanges
            await UoW.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateResidentAsync(Guid userId, Guid activatedBy)
        {
            // Mutates in memory
            await UoW.Residents.SetResidentActiveStatusAsync(userId, true, activatedBy);

            // ONE SaveChanges
            await UoW.SaveChangesAsync();
            return true;
        }
    }
}

*/


































/*using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
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
*/











