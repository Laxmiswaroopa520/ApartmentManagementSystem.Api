
using ApartmentManagementSystem.Application.DTOs.Dashboard;

namespace ApartmentManagementSystem.Application.Interfaces.Services;

public interface IEnhancedDashboardService
{
    Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId);
    Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId);
    Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync();
    Task<FinancialSummaryDto> GetFinancialSummaryAsync();

    Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role);
    // Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role);
}
