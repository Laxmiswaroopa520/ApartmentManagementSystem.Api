namespace ApartmentManagementSystem.Domain.Constants
{
    public static class SystemRoles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Manager = "Manager"; 
        public const string President = "President";
        public const string Staff = "Staff";
        public const string Secretary = "Secretary";
        public const string Treasurer = "Treasurer";
        public const string ResidentOwner = "ResidentOwner";
        public const string Tenant = "Tenant";
        public const string Maintenance = "Maintenance";


        // Staff Roles
        public const string Security = "Security";

        public const string Plumber = "Plumber";
        public const string Electrician = "Electrician";
        public const string Carpenter = "Carpenter";
        public const string Sweeper = "Sweeper";
        public const string Gardener = "Gardener";
        public const string MaintenanceStaff = "MaintenanceStaff";

        public const string AdminManagerCommunity =
           $"{SuperAdmin},{Manager},{President},{Secretary},{Treasurer}";
        public static readonly IReadOnlyList<string> StaffRoles = new[]
      {
            Security,
            Plumber,
            Electrician,
            Carpenter,
            Sweeper,
            Gardener,
            MaintenanceStaff
        };
        public const string StaffRolesString =
     Security + "," +
     Plumber + "," +
     Electrician + "," +
     Carpenter + "," +
     Sweeper + "," +
     Gardener + "," +
     MaintenanceStaff;
    }
}

