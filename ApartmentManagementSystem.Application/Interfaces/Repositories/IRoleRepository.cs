using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id);
        Task<Role?> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
    }
}
