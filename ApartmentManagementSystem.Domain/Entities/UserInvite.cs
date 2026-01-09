using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Entities;

public class UserInvite
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PrimaryPhone { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Guid? FlatId { get; set; } // Will be used in Phase 3
    public string InviteStatus { get; set; } = "Pending"; // Pending, Verified, Completed
    public DateTime CreatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
    public Guid UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    // Navigation
    public Role Role { get; set; } = null!;
}