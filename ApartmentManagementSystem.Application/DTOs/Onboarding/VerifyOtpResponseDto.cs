using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
   // namespace ApartmentManagementSystem.Application.DTOs.Onboarding;

    public class VerifyOtpResponseDto
    {
        //public Guid UserId { get; set; }
        public bool IsVerified { get; set; }
        public string PrimaryPhone { get; set; } = string.Empty;
    }
}