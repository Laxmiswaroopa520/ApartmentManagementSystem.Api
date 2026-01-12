using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid id);

        Task<List<Role>> GetAllAsync();
    }
}
