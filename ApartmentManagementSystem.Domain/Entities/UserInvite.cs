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

    public bool IsOtpVerified { get; set; }     //imp


    public Guid RoleId { get; set; }
    public Guid? FlatId { get; set; }

    public string InviteStatus { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }

    public Guid UserId { get; set; }   // will be empty GUID until user completes onboarding

    public DateTime ExpiresAt { get; set; }

    // Navigation
    public Role Role { get; set; } = null!;
}
