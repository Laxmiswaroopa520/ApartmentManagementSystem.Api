
using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFloorRepository
    {
        Task<Floor?> GetByIdAsync(Guid id);
        Task<List<Floor>> GetAllAsync();
        Task<List<Floor>> GetByApartmentIdAsync(Guid apartmentId); // ⭐ NEW
        Task AddAsync(Floor floor);
        Task UpdateAsync(Floor floor);
        Task DeleteAsync(Floor floor);
        Task SaveChangesAsync();
    }
}