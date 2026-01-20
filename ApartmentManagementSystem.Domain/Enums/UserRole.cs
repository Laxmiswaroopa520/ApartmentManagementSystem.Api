namespace ApartmentManagementSystem.Domain.Enums;

public static class UserRole
{
    // SYSTEM ROLES (Platform level)
    public const string SuperAdmin = "SuperAdmin";
    public const string Manager = "Manager";
    
    // COMMUNITY ROLES (Assigned by Admin - NOT during onboarding)
    public const string President = "President";
    public const string Secretary = "Secretary";
    public const string Treasurer = "Treasurer";
    
    // RESIDENT ROLES (Selected during onboarding)
    public const string ResidentOwner = "ResidentOwner";
    public const string Tenant = "Tenant";
    
    // STAFF ROLES
    public const string Security = "Security";
    public const string MaintenanceStaff = "MaintenanceStaff";
}

