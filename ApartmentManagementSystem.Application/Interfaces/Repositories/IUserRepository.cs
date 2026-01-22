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


       
            // YOUR EXISTING METHODS
            Task<User?> GetByUsernameAsync(string username);
            Task<User?> GetByIdAsync(Guid id);
            Task<User?> GetByPhoneAsync(string phone);
            Task AddAsync(User user);
            Task SaveChangesAsync();
            Task<List<User>> GetPendingResidentsAsync();

            // ADDED: Missing methods needed by services
            Task<User?> GetByEmailAsync(string email);
            Task<bool> PhoneExistsAsync(string phone);
            Task<bool> UsernameExistsAsync(string username);
            Task UpdateAsync(User user);
        Task<User?> GetByUsernameWithRolesAsync(string username);//added new method
    }

    }
