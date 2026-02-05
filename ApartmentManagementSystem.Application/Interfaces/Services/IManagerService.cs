using ApartmentManagementSystem.Application.DTOs.Manager;
using System;
using System.Collections.Generic;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IManagerService
    {
        /// <summary>
        /// ⭐ NEW CORRECTED: Get ResidentOwners from THIS apartment who can be made managers
        /// </summary>
        Task<List<AvailableManagerDto>> GetApartmentResidentsForManagerAssignmentAsync(Guid apartmentId);

        /// <summary>
        /// Assign manager to apartment - supports both resident and external
        /// </summary>
        Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(AssignManagerRequestDto dto, Guid assignedBy);

        /// <summary>
        /// Remove manager from apartment
        /// </summary>
        Task<bool> RemoveManagerFromApartmentAsync(RemoveManagerRequestDto dto, Guid removedBy);

        /// <summary>
        /// Get all current managers
        /// </summary>
        Task<List<ManagerListDto>> GetAllManagersAsync();

        /// <summary>
        /// Get manager details by user ID
        /// </summary>
        Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId);
    }
}