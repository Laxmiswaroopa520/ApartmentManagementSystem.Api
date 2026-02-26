using ApartmentManagementSystem.Application.DTOs.Dashboard;
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync(Guid userId);
        Task<OwnerDashboardDto> GetOwnerDashboardAsync(Guid userId);
        Task<TenantDashboardDto> GetTenantDashboardAsync(Guid userId);
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
