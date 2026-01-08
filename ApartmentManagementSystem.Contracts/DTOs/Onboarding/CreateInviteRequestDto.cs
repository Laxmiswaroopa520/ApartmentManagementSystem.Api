using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Contracts.DTOs.Onboarding
{
    public class CreateInviteRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public Guid RoleId { get; set; }
    }
}