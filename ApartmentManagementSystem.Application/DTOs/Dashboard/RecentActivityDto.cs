namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class RecentActivityDto
    {
        public string Activity { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
