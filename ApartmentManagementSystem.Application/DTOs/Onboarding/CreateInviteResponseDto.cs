using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DateTime ExpiresAt { get; set; }
    }

}