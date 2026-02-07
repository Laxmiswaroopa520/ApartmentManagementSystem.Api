// ApartmentManagementSystem.Application/Interfaces/Repositories/IEnhancedDashboardRepository.cs
/*
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
*/
using ApartmentManagementSystem.Application.DTOs.Dashboard;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IEnhancedDashboardRepository
    {
        Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync();
        Task<ApartmentDashboardStatsDto> GetApartmentDashboardStatsAsync(Guid apartmentId);
        Task<FinancialSummaryDto> GetFinancialSummaryAsync();
        Task<FinancialSummaryDto> GetApartmentFinancialSummaryAsync(Guid apartmentId);
        Task<List<RecentActivityDto>> GetRecentActivitiesAsync();
        Task<List<RecentActivityDto>> GetApartmentRecentActivitiesAsync(Guid apartmentId);
        Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId);
        Task PopulateApartmentStatsAsync(AdvancedDashboardStatsDto stats);
        Task<List<NoticeBoardMessageDto>> GetNoticeBoardMessagesAsync(Guid apartmentId);
    }
}