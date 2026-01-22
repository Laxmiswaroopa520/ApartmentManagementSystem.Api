using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// these are constant role names not stored in database..
namespace ApartmentManagementSystem.Domain.Enums
{

    public static class RoleNames
    {
        // SYSTEM ROLES (Platform level)
        public const string SuperAdmin = "SuperAdmin";
        public const string Manager = "Manager";

        // COMMUNITY ROLES (Assigned by Admin/Manager - NOT during onboarding)
        public const string President = "President";
        public const string Secretary = "Secretary";
        public const string Treasurer = "Treasurer";

        // RESIDENT ROLES (Selected during onboarding)
        public const string ResidentOwner = "ResidentOwner";
        public const string Tenant = "Tenant";

        // STAFF ROLES (Created by Admin/Manager/Community Members)
        public const string Security = "Security";
        public const string Plumber = "Plumber";
        public const string Electrician = "Electrician";
        public const string Carpenter = "Carpenter";
        public const string Sweeper = "Sweeper";
        public const string Gardener = "Gardener";
        public const string MaintenanceStaff = "MaintenanceStaff";

        public static List<string> GetAllRoles()
        {
            return new List<string>
        {
            SuperAdmin, Manager, President, Secretary, Treasurer,
            ResidentOwner, Tenant, Security, Plumber, Electrician,
            Carpenter, Sweeper, Gardener, MaintenanceStaff
        };
        }

        public static List<string> GetCommunityRoles()
        {
            return new List<string> { President, Secretary, Treasurer };
        }

        public static List<string> GetStaffRoles()
        {
            return new List<string>
        {
            Security, Plumber, Electrician, Carpenter,
            Sweeper, Gardener, MaintenanceStaff
        };
        }

        public static List<string> GetAdminRoles()
        {
            return new List<string> { SuperAdmin, Manager, President, Secretary, Treasurer };
        }
    }
}