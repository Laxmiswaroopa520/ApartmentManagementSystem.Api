using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    using ApartmentManagementSystem.Application.DTOs.Auth;
  //  using global::ApartmentManagementSystem.Application.DTOs.Auth;

    //namespace ApartmentManagementSystem.Application.Interfaces.Services;

    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}