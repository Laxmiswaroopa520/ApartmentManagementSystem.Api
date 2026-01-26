// ApartmentManagementSystem.Application/Interfaces/Repositories/IEnhancedDashboardRepository.cs
using ApartmentManagementSystem.Application.DTOs.Dashboard;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories;

public interface IEnhancedDashboardRepository
{
    Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync();
    Task<FinancialSummaryDto> GetFinancialSummaryAsync();
    Task<List<RecentActivityDto>> GetRecentActivitiesAsync();
    Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId);
    Task PopulateApartmentStatsAsync(AdvancedDashboardStatsDto stats);          //added for apartments features in hte super admin dashboard
}
