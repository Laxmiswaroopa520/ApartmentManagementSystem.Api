/*
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
*/

using ApartmentManagementSystem.Application.DTOs.Dashboard;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IEnhancedDashboardService
    {
        Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId);
        Task<ManagerDashboardDto> GetManagerDashboardAsync(Guid userId);
        Task<CommunityLeaderDashboardDto> GetCommunityLeaderDashboardAsync(Guid userId, string role);
        Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId);
        Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync();
        Task<ApartmentDashboardStatsDto> GetApartmentDashboardStatsAsync(Guid apartmentId);
        Task<FinancialSummaryDto> GetFinancialSummaryAsync();
        Task<FinancialSummaryDto> GetApartmentFinancialSummaryAsync(Guid apartmentId);
        Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role);
        Task<List<NoticeBoardMessageDto>> GetNoticeBoardMessagesAsync(Guid apartmentId);
    }
}