using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Constants
{
    public static class SystemRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Manager = "Manager"; //  NEW
        public const string President = "President";
        public const string Staff = "Staff";//optional
        public const string Secretary = "Secretary";
        public const string Treasurer = "Treasurer";
        public const string ResidentOwner = "ResidentOwner";
        public const string Tenant = "Tenant";
        public const string Security = "Security";
        public const string Maintenance = "Maintenance";
    }
}
