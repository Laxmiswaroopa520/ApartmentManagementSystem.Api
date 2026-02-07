using ApartmentManagementSystem.Application.DTOs.Admin;

namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class ManagerDashboardDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid ApartmentId { get; set; }
        public string ApartmentName { get; set; } = string.Empty;
        public ApartmentDashboardStatsDto Stats { get; set; } = new();
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
        public List<QuickActionDto> QuickActions { get; set; } = new();
        public List<NoticeBoardMessageDto> NoticeBoard { get; set; } = new();
        public List<PendingResidentDto> PendingResidents { get; set; } = new();
    }
}