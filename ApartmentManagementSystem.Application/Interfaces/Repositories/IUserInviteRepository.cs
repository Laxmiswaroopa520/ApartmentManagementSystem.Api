

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;
    using System.Threading.Tasks;
    public interface IUserInviteRepository
    {
        Task<UserInvite?> GetByIdAsync(Guid id);
        Task<UserInvite?> GetByPhoneAsync(string phone);
        Task<UserInvite> CreateAsync(UserInvite invite); 
        Task UpdateStatusAsync(Guid inviteId, string status); 
        Task<List<UserInvite>> GetAllAsync();
        Task SaveChangesAsync();

    }
}