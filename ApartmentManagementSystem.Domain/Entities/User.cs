using ApartmentManagementSystem.Domain.Enums;
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
    //  public Guid RoleId { get; set; }

    // NEW: Registration workflow tracking
    public ResidentStatus Status { get; set; } = ResidentStatus.PendingOtpVerification;
    public ResidentType? ResidentType { get; set; }

    public bool IsActive { get; set; }
    public bool IsOtpVerified { get; set; }
    public bool IsRegistrationCompleted { get; set; }

    // Flat assignment (nullable - assigned later by admin)
    public Guid? FlatId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation
    //  public Role Role { get; set; } = null!;
    public Flat? Flat { get; set; }
    /* public ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();
     public ICollection<UserFlatMapping> UserFlatMappings { get; set; } = new List<UserFlatMapping>();
     // i added this becuase 1 user can have multiple roels right?? like owner+president liek that..
     public ICollection<RoleNames> UserRoles { get; set; } = new List<RoleNames>();
    */
    //
       // I Added this becuase 1 user can have multiple roels right?? like owner+president liek that.. and also many roles has many users like 2 securities like that..

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();
    public ICollection<UserFlatMapping> UserFlatMappings { get; set; } = new List<UserFlatMapping>();
}



























/*
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

    // NEW: Phase 2 relationships
    public ICollection<UserFlatMapping> UserFlatMappings { get; set; } = new List<UserFlatMapping>();
    public ICollection<Flat> OwnedFlats { get; set; } = new List<Flat>();
   // public ICollection<Flat> RentedFlats { get; set; } = new List<Flat>();
}
*/