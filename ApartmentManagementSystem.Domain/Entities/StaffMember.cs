using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Entities
{
   public class StaffMember
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string StaffType { get; set; } = string.Empty; // Security, Plumber, etc.
        public string? Specialization { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime JoinedOn { get; set; } = DateTime.UtcNow;

        // If staff has login access
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        // Audit fields
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
