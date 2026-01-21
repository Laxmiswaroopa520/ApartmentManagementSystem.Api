using ApartmentManagementSystem.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IEnhancedDashboardService
    {
        Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId);
        Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId);
        Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync();
        Task<FinancialSummaryDto> GetFinancialSummaryAsync();
        Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role);
    }
}
