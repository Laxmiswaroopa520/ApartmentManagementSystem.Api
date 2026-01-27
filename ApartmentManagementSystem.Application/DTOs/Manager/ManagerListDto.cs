using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Manager
{
    public class ManagerListDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid? ApartmentId { get; set; }
        public string? ApartmentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? AssignedAt { get; set; }
    }
}
