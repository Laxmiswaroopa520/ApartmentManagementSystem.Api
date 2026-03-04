namespace ApartmentManagementSystem.Domain.Entities;
public class UserOtp
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;

    public string OtpCode { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }
}



