using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Entities
{
    public class UserFlatMapping
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FlatId { get; set; }
        public string RelationshipType { get; set; } = string.Empty; // "Owner" or "Tenant"
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User User { get; set; } = null!;
        public Flat Flat { get; set; } = null!;
    }
}
