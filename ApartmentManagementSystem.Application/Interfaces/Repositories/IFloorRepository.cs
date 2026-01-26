using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFloorRepository
    {
        Task<List<Floor>> GetAllAsync();
    }
}
*/
namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFloorRepository
    {
        Task<Floor?> GetByIdAsync(Guid id);
        Task<List<Floor>> GetByApartmentIdAsync(Guid apartmentId);
        Task AddAsync(Floor floor);
        Task UpdateAsync(Floor floor);
    }
}