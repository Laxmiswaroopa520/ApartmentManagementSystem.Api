using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Community
{
    public class CreateStaffMemberDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string StaffType { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Password { get; set; } // Optional for login

    }
}
