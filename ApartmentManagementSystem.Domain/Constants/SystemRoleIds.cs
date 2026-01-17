using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Constants
{
    public static class SystemRoleIds
    {
        public static readonly Guid SuperAdmin = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid President = Guid.Parse("10000000-0000-0000-0000-000000000002");
        public static readonly Guid Secretary = Guid.Parse("10000000-0000-0000-0000-000000000003");
        public static readonly Guid Treasurer = Guid.Parse("10000000-0000-0000-0000-000000000004");

        public static readonly Guid ResidentOwner = Guid.Parse("10000000-0000-0000-0000-000000000005");
        public static readonly Guid Tenant = Guid.Parse("10000000-0000-0000-0000-000000000006");
        public static readonly Guid Security = Guid.Parse("10000000-0000-0000-0000-000000000007");
        public static readonly Guid Maintenance = Guid.Parse("10000000-0000-0000-0000-000000000008");
    }
}
