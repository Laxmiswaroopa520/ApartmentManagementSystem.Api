

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;

 //   namespace ApartmentManagementSystem.Application.Interfaces.Repositories;

    public interface IUserInviteRepository
    {
        Task AddAsync(UserInvite invite);
        Task SaveChangesAsync();
    }
}