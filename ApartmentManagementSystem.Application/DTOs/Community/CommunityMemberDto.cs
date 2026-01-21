using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Community
{
    public class CommunityMemberDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FlatNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime AssignedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
