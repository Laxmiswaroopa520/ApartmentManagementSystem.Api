using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//used in the ui for storing full name,mobile number and otp..
namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    /*    public class CreateInviteResponseDto
        {
            public Guid UserId { get; set; }
            public string PrimaryPhone { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
        }*/
    // namespace ApartmentManagementSystem.Application.DTOs.Onboarding;

    public class CreateInviteResponseDto
    {
        public Guid InviteId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime OtpExpiresAt { get; set; }
    }


}