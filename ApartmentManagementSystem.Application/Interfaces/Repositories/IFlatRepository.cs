using ApartmentManagementSystem.Domain.Entities;
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


