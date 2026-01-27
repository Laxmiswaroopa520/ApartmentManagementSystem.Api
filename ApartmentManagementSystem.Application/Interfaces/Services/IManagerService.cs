using ApartmentManagementSystem.Application.DTOs.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IManagerService
    {
        Task<List<AvailableManagerDto>> GetAvailableManagersAsync(Guid apartmentId);
        Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(AssignManagerRequestDto dto, Guid assignedBy);
        Task<bool> RemoveManagerFromApartmentAsync(RemoveManagerRequestDto dto, Guid removedBy);
        Task<List<ManagerListDto>> GetAllManagersAsync();
        Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId);
    }
}