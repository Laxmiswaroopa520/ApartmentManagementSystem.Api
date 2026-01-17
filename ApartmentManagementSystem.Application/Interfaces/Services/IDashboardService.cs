using ApartmentManagementSystem.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
