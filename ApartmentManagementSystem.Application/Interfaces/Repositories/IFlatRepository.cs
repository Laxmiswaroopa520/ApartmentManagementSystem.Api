using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFlatRepository
    {
        Task<Flat?> GetByIdAsync(Guid id);
        Task<List<Flat>> GetByUserIdAsync(Guid userId);
        Task<int> GetTotalCountAsync();
        Task<int> GetOccupiedCountAsync();



        Task<List<Flat>> GetFlatsWithMappingsByOwnerIdAsync(Guid ownerUserId);

    }
}
