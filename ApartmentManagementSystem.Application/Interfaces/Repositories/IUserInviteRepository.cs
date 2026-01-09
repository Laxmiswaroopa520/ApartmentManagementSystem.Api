using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;
    using global::ApartmentManagementSystem.Domain.Entities;

 //   namespace ApartmentManagementSystem.Application.Interfaces.Repositories;

    public interface IUserInviteRepository
    {
        Task AddAsync(UserInvite invite);
    }
}