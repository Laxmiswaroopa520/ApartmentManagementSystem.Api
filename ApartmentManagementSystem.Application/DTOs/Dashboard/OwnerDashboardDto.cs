using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class OwnerDashboardDto
    {
        public string FullName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public List<FlatSummaryDto> MyFlats { get; set; } = new();
        public int PendingComplaints { get; set; }
        public int PendingBills { get; set; }
        public decimal TotalOutstanding { get; set; }
    }
}
