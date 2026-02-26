namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class EnhancedAdminDashboardDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<string> AllRoles { get; set; } = new();
        public AdvancedDashboardStatsDto Stats { get; set; } = new();
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public FinancialSummaryDto? FinancialSummary { get; set; }
        public List<QuickActionDto> QuickActions { get; set; } = new();
        public List<UpcomingEventDto> UpcomingEvents { get; set; } = new();
    }

}
