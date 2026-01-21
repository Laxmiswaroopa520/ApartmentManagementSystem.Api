using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Community.DashboardAnalytics
{
    public class MonthlyCollectionDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
