using ApartmentManagementSystem.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApartmentManagementSystem.Application.Interfaces.Services;
namespace ApartmentManagementSystem.Infrastructure.OTP
{
    public class OtpService : IOtpService
    {
        public string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public Task SendOtpAsync(string mobile, string otp)
        {
            Console.WriteLine($"OTP sent to {mobile}: {otp}");
            return Task.CompletedTask;
        }
    }
}