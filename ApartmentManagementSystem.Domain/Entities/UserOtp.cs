using ApartmentManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApartmentManagementSystem.Domain.Entities;

public class UserOtp
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OtpCode { get; set; } = string.Empty;
    public DateTime ExpiryTime { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}