using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
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
*/
using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Onboarding;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IAdminResidentService
    {
        Task<List<PendingResidentDto>> GetPendingResidentsAsync();
        Task<AssignFlatResponseDto> AssignFlatToResidentAsync(AssignFlatDto dto);

        // ⭐ NEW: Multi-apartment support
        Task<List<ApartmentDropdownDto>> GetApartmentsForUserAsync(Guid userId, string role);
        Task<List<FloorDto>> GetFloorsByApartmentAsync(Guid apartmentId);
        Task<List<FlatDto>> GetVacantFlatsByFloorAsync(Guid floorId);

        // ⭐ DEPRECATED: Remove these old methods
        // Task<List<FloorDto>> GetAllFloorsAsync();
    }
}
