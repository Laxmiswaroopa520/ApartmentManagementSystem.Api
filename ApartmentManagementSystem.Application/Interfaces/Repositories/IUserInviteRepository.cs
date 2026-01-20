

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;
    using System.Threading.Tasks;

    //   namespace ApartmentManagementSystem.Application.Interfaces.Repositories;

    /* public interface IUserInviteRepository
     {
         Task<UserInvite?> GetByIdAsync(Guid id);
         Task<UserInvite?> GetByPhoneAsync(string phone);
         Task AddAsync(UserInvite invite);
         Task SaveChangesAsync();
         Task<UserInvite> CreateAsync(UserInvite invite); // ADDED
         Task UpdateStatusAsync(Guid inviteId, string status);
     }*/
    public interface IUserInviteRepository
    {
        Task<UserInvite?> GetByIdAsync(Guid id);
        Task<UserInvite?> GetByPhoneAsync(string phone);
        Task<UserInvite> CreateAsync(UserInvite invite); // ADDED
        Task UpdateStatusAsync(Guid inviteId, string status); // ADDED
        Task<List<UserInvite>> GetAllAsync();
        Task SaveChangesAsync();

    }
}