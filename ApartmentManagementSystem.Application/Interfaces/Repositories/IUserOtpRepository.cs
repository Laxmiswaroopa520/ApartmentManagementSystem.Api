using ApartmentManagementSystem.Domain.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IUserOtpRepository
    {
        Task AddAsync(UserOtp otp);
        Task<UserOtp?> GetValidOtpAsync(string email, string otp);
        Task UpdateAsync(UserOtp otp);

    }
}