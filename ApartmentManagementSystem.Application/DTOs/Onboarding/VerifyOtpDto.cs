using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
  //  namespace ApartmentManagementSystem.Application.DTOs.Onboarding;

    public class VerifyOtpDto
    {
        public string PrimaryPhone { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}