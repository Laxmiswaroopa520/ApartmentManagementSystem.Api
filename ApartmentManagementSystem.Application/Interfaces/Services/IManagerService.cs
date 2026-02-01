using ApartmentManagementSystem.Application.DTOs.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IManagerService
    {
        /// <summary>All users with Manager role (excluding the one already on this apartment)</summary>
        Task<List<AvailableManagerDto>> GetAvailableManagersAsync(Guid apartmentId);

        /// <summary>⭐ NEW: Only Manager-role users who are residents of THIS apartment</summary>
        Task<List<AvailableManagerDto>> GetResidentManagersForApartmentAsync(Guid apartmentId);

        Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(AssignManagerRequestDto dto, Guid assignedBy);
        Task<bool> RemoveManagerFromApartmentAsync(RemoveManagerRequestDto dto, Guid removedBy);
        Task<List<ManagerListDto>> GetAllManagersAsync();
        Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId);
    }
}*/

using ApartmentManagementSystem.Application.DTOs.Manager;

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