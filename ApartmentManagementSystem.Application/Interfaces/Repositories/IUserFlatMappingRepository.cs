using ApartmentManagementSystem.Domain.Entities;
namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IUserFlatMappingRepository
    {
        Task<UserFlatMapping?> GetByIdAsync(Guid id);
        Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId);
        Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId);
        Task<UserFlatMapping?> GetActiveMappingByUserIdAsync(Guid userId);
        Task<UserFlatMapping?> GetActiveMappingByFlatIdAsync(Guid flatId);
        Task AddAsync(UserFlatMapping mapping);
        Task UpdateAsync(UserFlatMapping mapping);
        Task SaveChangesAsync();
    }
}

