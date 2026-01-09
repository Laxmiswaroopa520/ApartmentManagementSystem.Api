using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
   // namespace ApartmentManagementSystem.Application.DTOs.Onboarding;

    public class CompleteRegistrationDto
    {
        public string PrimaryPhone { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? SecondaryPhone { get; set; }
        public string? Email { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}