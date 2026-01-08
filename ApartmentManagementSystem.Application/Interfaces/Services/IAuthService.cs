using ApartmentManagementSystem.Application.DTOs;
using ApartmentManagementSystem.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IAuthService
    {
        // Task<LoginResponse> LoginAsync(string email, string password);
        //<string> LoginAsync(string email, string password);
        Task<LoginResult> LoginAsync(LoginCommand command);
    }
}