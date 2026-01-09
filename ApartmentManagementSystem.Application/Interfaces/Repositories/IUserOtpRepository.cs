using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;
    using global::ApartmentManagementSystem.Domain.Entities;

   // namespace ApartmentManagementSystem.Application.Interfaces.Repositories;

    public interface IUserOtpRepository
    {
        Task<UserOtp?> GetValidOtpAsync(Guid userId, string otp);
        Task AddAsync(UserOtp otp);
        Task MarkAsUsedAsync(Guid otpId);
    }
}