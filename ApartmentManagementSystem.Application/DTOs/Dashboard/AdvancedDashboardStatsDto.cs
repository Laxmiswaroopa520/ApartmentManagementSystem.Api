using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class AdvancedDashboardStatsDto
    {
        public int TotalResidents { get; set; }
        public int TotalFlats { get; set; }
        public int OccupiedFlats { get; set; }
        public int VacantFlats { get; set; }
        public int PendingRegistrations { get; set; }
        public int TotalStaffMembers { get; set; }
        public int ActiveStaffMembers { get; set; }
        public int CommunityMembers { get; set; }
        public int PendingComplaints { get; set; }
        public int ResolvedComplaintsThisMonth { get; set; }
        public decimal TotalOutstandingBills { get; set; }
        public decimal CollectionThisMonth { get; set; }
        public int TodaysVisitors { get; set; }
        public int ActiveSecurityPersonnel { get; set; }
    }
}
