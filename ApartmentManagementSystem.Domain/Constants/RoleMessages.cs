//for fast end points messages
using System.Reflection.Metadata;

namespace ApartmentManagementSystem.Domain.Constants
{
    /// <summary>
    /// Contains constant messages related to system roles.
    /// </summary>
    public static class RoleMessages
    {
        /// <summary>
        /// Swagger description for GetRoles endpoint.
        /// </summary>
        public const string GetRolesDescription =
            "Returns a list of all roles available in the system. Used by web interface for role selection.";
        public const string SuperAdminMessage=  "Super Administrator with full system access";
        public const string ManagerSeedingSuccess = "Manager with administrative privileges";
        public const string PresidentSeededSuccess="Community President — Leadership role";
        public const string SecretarySeededMessage="Community Secretary — Administrative role";
        public const string TreasurerSeededSuccess= "Community Treasurer — Financial management role";
        public const string OwnerSeedingSuccess="Resident Owner of the flat";
        public const string TenantSeedingMessage= "Tenant renting the flat";
        public const string SecuritySeedingSuccess="Security personnel";
        public const string PlumberSeeded= "Plumber staff";
        public const string ElectricianSeeded= "Electrician staff";
        public const string CarpenterSeeded= "Carpenter staff";
        public const string CleaningStaffSeeded="Cleaning staff";
        public const string GardeningStaffSeeded="Gardening staff";
        public const string MaintenanceStaffSeeded="General maintenance staff";
        public const string SeedRolesMessage= "SeedRolesAsync() must complete successfully before SeedSuperAdminAsync().";
    }
}