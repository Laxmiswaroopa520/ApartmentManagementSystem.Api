using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



/*
    namespace ApartmentManagementSystem.Application.Interfaces.Repositories
    {
        public interface IFlatRepository
        {
            Task<Flat?> GetByIdAsync(Guid id);
        //  Task<Flat> UpdateAsync(Flat flat);
        Task UpdateAsync(Flat flat);

        Task<List<Flat>> GetByUserIdAsync(Guid userId);

            // OWNER DASHBOARD
            Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerUserId);

            // ADMIN / STATS
            Task<int> GetTotalCountAsync();
            Task<int> GetOccupiedCountAsync();

        // ONBOARDING
        Task<Flat?> GetFlatByResidentIdAsync(Guid userId); //newly added
        Task<List<Floor>> GetAllFloorsAsync();
        Task<List<Flat>> GetAvailableFlatsByFloorAsync(Guid floorId);
        Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId);
        Task SaveChangesAsync();
        }
    }
*/
namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFlatRepository
    {
        Task<Flat?> GetByIdAsync(Guid id);
        Task<List<Flat>> GetByFloorIdAsync(Guid floorId);
        Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId);
        Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerId);
        Task<List<Floor>> GetAllFloorsAsync();
        Task<int> GetTotalCountAsync();
        Task<int> GetOccupiedCountAsync();
        Task AddAsync(Flat flat);
        Task UpdateAsync(Flat flat);
        Task SaveChangesAsync();
    }
}


