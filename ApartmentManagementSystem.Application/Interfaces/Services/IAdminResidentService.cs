using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
   public interface IAdminResidentService
    {
        Task<List<PendingResidentDto>> GetPendingResidentsAsync();
        Task<AssignFlatResponseDto> AssignFlatToResidentAsync(AssignFlatDto dto);
        Task<List<FloorDto>> GetAllFloorsAsync();
        Task<List<FlatDto>> GetVacantFlatsByFloorAsync(Guid floorId);

    }
}
