using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IResidentManagementService
    {
        Task<List<ResidentListDto>> GetAllResidentsAsync();
        Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType);
        Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId);
        Task<bool> DeactivateResidentAsync(Guid userId, Guid deactivatedBy);
        Task<bool> ActivateResidentAsync(Guid userId, Guid activatedBy);
    }
}
