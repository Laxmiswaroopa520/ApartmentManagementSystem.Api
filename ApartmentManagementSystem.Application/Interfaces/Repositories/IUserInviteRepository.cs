using ApartmentManagementSystem.Domain.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IUserInviteRepository
    {
        Task AddAsync(UserInvite invite);
        Task<UserInvite?> GetValidInviteAsync(string email);
        Task UpdateAsync(UserInvite invite);
    }
}