using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    using ApartmentManagementSystem.Application.DTOs.Auth;
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
       //used for checking inactive users..
        Task<bool> IsUserActiveAsync(Guid userId);
    }
}