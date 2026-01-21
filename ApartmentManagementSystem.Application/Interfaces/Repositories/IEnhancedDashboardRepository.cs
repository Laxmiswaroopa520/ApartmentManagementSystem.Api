using ApartmentManagementSystem.Application.DTOs.Dashboard;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IEnhancedDashboardRepository
    {
        Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync();
        Task<FinancialSummaryDto> GetFinancialSummaryAsync();
        Task<List<RecentActivityDto>> GetRecentActivitiesAsync();
        Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId);
    }
}
