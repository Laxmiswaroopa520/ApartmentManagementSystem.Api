using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IUserInviteRepository : IGenericRepository<UserInvite>
    {
        Task<UserInvite?> GetByIdWithRoleAsync(Guid id);
        Task<UserInvite?> GetByPhoneAsync(string phone);
        Task UpdateStatusAsync(Guid inviteId, string status); // mutates in memory, UoW saves
    }
}



/*namespace ApartmentManagementSystem.Application.Interfaces.Repositories
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
*/