using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    /* public interface IFlatRepository
     {
         Task<Flat?> GetByIdAsync(Guid id);
         Task<List<Flat>> GetByUserIdAsync(Guid userId);
         Task<int> GetTotalCountAsync();
         Task<int> GetOccupiedCountAsync();



         Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerUserId);

     }

     */

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
        Task<List<Floor>> GetAllFloorsAsync();
        Task<List<Flat>> GetAvailableFlatsByFloorAsync(Guid floorId);
        Task<List<Flat>> GetVacantFlatsByFloorAsync(Guid floorId);
        Task SaveChangesAsync();
        }
    }


