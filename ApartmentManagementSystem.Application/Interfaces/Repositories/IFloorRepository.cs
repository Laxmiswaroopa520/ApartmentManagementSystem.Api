using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFloorRepository : IGenericRepository<Floor>
    {
        Task<Floor?> GetByIdWithDetailsAsync(Guid id);
        Task<List<Floor>> GetByApartmentIdAsync(Guid apartmentId);
    }
}



/*using ApartmentManagementSystem.Domain.Entities;
namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IFloorRepository
    {
        Task<Floor?> GetByIdAsync(Guid id);
        Task<List<Floor>> GetAllAsync();
        Task<List<Floor>> GetByApartmentIdAsync(Guid apartmentId); 
        Task AddAsync(Floor floor);
        Task UpdateAsync(Floor floor);
        Task DeleteAsync(Floor floor);
        Task SaveChangesAsync();
    }
}
*/