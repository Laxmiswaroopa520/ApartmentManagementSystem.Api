/*using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IUserOtpRepository
    {
        Task<UserOtp?> GetValidOtpAsync(string phoneNumber, string otp);
        Task AddAsync(UserOtp otp);
        Task MarkAsUsedAsync(Guid otpId);
    }
}
*/
using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IUserOtpRepository : IGenericRepository<UserOtp>
    {
        Task<UserOtp?> GetValidOtpAsync(string phone, string otp);
        Task MarkAsUsedAsync(Guid otpId); // mutates in memory, UoW saves
    }
}
