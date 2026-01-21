using ApartmentManagementSystem.Application.DTOs.Community.DashboardAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class FinancialSummaryDto
    {
        public decimal TotalOutstanding { get; set; }
        public decimal CollectedThisMonth { get; set; }
        public decimal CollectedLastMonth { get; set; }
        public decimal PendingMaintenanceFees { get; set; }
        public decimal PendingUtilityBills { get; set; }
        public List<MonthlyCollectionDto> Last6MonthsCollection { get; set; } = new();
    }

}
