
// ApartmentManagementSystem.Domain/Entities/User.cs
namespace ApartmentManagementSystem.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? Email { get; set; }
    public string PrimaryPhone { get; set; } = string.Empty;
    public string? SecondaryPhone { get; set; }
    public Guid RoleId { get; set; }
    public bool IsActive { get; set; }
    public bool IsOtpVerified { get; set; }
    public bool IsRegistrationCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public Role Role { get; set; } = null!;
    public ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();
}
