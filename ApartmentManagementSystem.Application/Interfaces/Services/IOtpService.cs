using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IOtpService
    {
        string GenerateOtp();

        Task SendOtpAsync(string mobile, string otp);
    }
}