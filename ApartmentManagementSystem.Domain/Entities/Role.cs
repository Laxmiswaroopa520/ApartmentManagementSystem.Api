namespace ApartmentManagementSystem.Domain.Entities;
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    //because 1 user can have many roles as well as 1 role can have many users..
    //1-user many roles and also many users are tenants,security like that ..
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}