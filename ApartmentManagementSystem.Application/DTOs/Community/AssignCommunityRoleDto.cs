using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Community
{
    public class AssignCommunityRoleDto
    {
        public Guid UserId { get; set; }
        public string CommunityRole { get; set; } = string.Empty; // President, Secretary, Treasurer
    }
}
