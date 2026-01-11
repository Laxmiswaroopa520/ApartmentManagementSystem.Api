using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;
 //   using global::ApartmentManagementSystem.Domain.Entities;

   // namespace ApartmentManagementSystem.Application.Interfaces.Repositories;

    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByPhoneAsync(string phone);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}