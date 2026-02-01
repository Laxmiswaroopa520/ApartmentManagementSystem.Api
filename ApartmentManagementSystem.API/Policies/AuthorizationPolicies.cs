using Microsoft.AspNetCore.Authorization;

namespace ApartmentManagementSystem.API.Policies
{
    public static class AuthorizationPolicies
    {
        public const string IsAdmin = "IsAdmin";
        public const string IsOwner = "IsOwner";
        public const string IsTenant = "IsTenant";
        public const string IsResident = "IsResident";
        public const string CanManageFlats = "CanManageFlats";
        public const string CanViewDashboard = "CanViewDashboard";
        public const string IsManager = "IsManager"; // ⭐ NEW

        public static void AddPolicies(AuthorizationOptions options)
        {
            // Admin policy
            options.AddPolicy(IsAdmin, policy =>
                policy.RequireRole("SuperAdmin", "President", "Secretary", "Treasurer"));

            // ⭐ NEW: Manager policy
            options.AddPolicy(IsManager, policy =>
                policy.RequireRole("Manager"));

            // Owner policy
            options.AddPolicy(IsOwner, policy =>
                policy.RequireRole("ResidentOwner"));

            // Tenant policy
            options.AddPolicy(IsTenant, policy =>
                policy.RequireRole("Tenant"));

            // Resident policy (Owner or Tenant)
            options.AddPolicy(IsResident, policy =>
                policy.RequireRole("ResidentOwner", "Tenant"));

            // Can manage flats (Admin roles + Manager)
            options.AddPolicy(CanManageFlats, policy =>
                policy.RequireRole("SuperAdmin", "Manager", "President", "Secretary"));

            // Can view dashboard
            options.AddPolicy(CanViewDashboard, policy =>
                policy.RequireRole("SuperAdmin", "Manager", "President", "Secretary", "Treasurer",
                                  "ResidentOwner", "Tenant"));
        }
    }
}


/*
namespace ApartmentManagementSystem.API.Policies
{
    public static class AuthorizationPolicies
    {
        public const string IsAdmin = "IsAdmin";
        public const string IsOwner = "IsOwner";
        public const string IsTenant = "IsTenant";
        public const string IsResident = "IsResident";
        public const string CanManageFlats = "CanManageFlats";
        public const string CanViewDashboard = "CanViewDashboard";

        public static void AddPolicies(AuthorizationOptions options)
        {
            // Admin policy (SuperAdmin, President, Secretary, Treasurer)
            options.AddPolicy(IsAdmin, policy =>
                policy.RequireRole("SuperAdmin", "President", "Secretary", "Treasurer"));

            // Owner policy
            options.AddPolicy(IsOwner, policy =>
                policy.RequireRole("ResidentOwner"));

            // Tenant policy
            options.AddPolicy(IsTenant, policy =>
                policy.RequireRole("Tenant"));

            // Resident policy (Owner or Tenant)
            options.AddPolicy(IsResident, policy =>
                policy.RequireRole("ResidentOwner", "Tenant"));

            // Can manage flats (Admin roles only)
            options.AddPolicy(CanManageFlats, policy =>
                policy.RequireRole("SuperAdmin", "President", "Secretary"));

            // Can view dashboard (Everyone except Security and Maintenance)
            options.AddPolicy(CanViewDashboard, policy =>
                policy.RequireRole("SuperAdmin", "President", "Secretary", "Treasurer",
                                  "ResidentOwner", "Tenant"));
        }
    }
}
*/