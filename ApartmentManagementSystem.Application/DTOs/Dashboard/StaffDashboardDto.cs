using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class StaffDashboardDto
    {
        public string FullName { get; set; } = string.Empty;
        public string StaffType { get; set; } = string.Empty;
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
        public int TodaysTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public List<TaskDto> MyTasks { get; set; } = new();
    }
}
