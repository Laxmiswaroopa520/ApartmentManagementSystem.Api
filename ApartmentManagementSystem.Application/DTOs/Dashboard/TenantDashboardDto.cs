namespace ApartmentManagementSystem.Application.DTOs.Dashboard
{
    public class TenantDashboardDto
    {
        public string FullName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public FlatSummaryDto? MyFlat { get; set; }
        public int PendingComplaints { get; set; }
        public decimal PendingRent { get; set; }
    }
}
