using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
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

    // NEW: Resident type instead of flat
    public ResidentType ResidentType { get; set; }

    public string InviteStatus { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }

    // Navigation
    public Role Role { get; set; } = null!;
}
