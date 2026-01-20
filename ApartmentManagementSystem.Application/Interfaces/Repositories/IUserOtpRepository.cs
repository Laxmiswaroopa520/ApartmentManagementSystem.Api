using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    using ApartmentManagementSystem.Domain.Entities;

   // namespace ApartmentManagementSystem.Application.Interfaces.Repositories;

    public interface IUserOtpRepository
    {
      //  Task<UserOtp?> GetLatestByUserIdAsync(Guid userId);
       // Task<UserOtp> CreateAsync(UserOtp otp);
        Task<UserOtp?> GetValidOtpAsync(string phoneNumber, string otp);
        Task AddAsync(UserOtp otp);
        Task MarkAsUsedAsync(Guid otpId);
    }
}