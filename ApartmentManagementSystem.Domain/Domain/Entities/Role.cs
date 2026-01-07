namespace ApartmentManagementSystem.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } // Admin, Owner, Tenant
}
