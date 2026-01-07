namespace ApartmentManagementSystem.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; }
    public string Mobile { get; set; }

    public string PasswordHash { get; set; }

    public bool IsActive { get; set; }
    public bool IsRegistrationCompleted { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }
}
