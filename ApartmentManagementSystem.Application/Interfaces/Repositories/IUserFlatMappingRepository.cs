using ApartmentManagementSystem.Domain.Entities;
namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IUserFlatMappingRepository
        {
            Task<List<UserFlatMapping>> GetByUserIdAsync(Guid userId);
            Task<List<UserFlatMapping>> GetByFlatIdAsync(Guid flatId);
        Task<UserFlatMapping> CreateAsync(UserFlatMapping mapping);
        Task AddAsync(UserFlatMapping mapping);
            Task SaveChangesAsync();
        }
    }

