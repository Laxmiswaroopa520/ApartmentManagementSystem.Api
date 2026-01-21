using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class EnhancedDashboardService : IEnhancedDashboardService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEnhancedDashboardRepository _dashboardRepo;

        public EnhancedDashboardService(
            IUserRepository userRepo,
            IEnhancedDashboardRepository dashboardRepo)
        {
            _userRepo = userRepo;
            _dashboardRepo = dashboardRepo;
        }

        public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            var stats = await _dashboardRepo.GetAdvancedDashboardStatsAsync();
            var recentActivities = await _dashboardRepo.GetRecentActivitiesAsync();
            var quickActions = GetQuickActionsForRole(user.Roles.First().Name);

            FinancialSummaryDto? financialSummary = null;
            if (user.Roles.Any(r =>
                r.Name == UserRole.SuperAdmin || r.Name == UserRole.Treasurer))
            {
                financialSummary = await _dashboardRepo.GetFinancialSummaryAsync();
            }

            return new EnhancedAdminDashboardDto
            {
                FullName = user.FullName,
                Role = user.Roles.First().Name,
                AllRoles = user.Roles.Select(r => r.Name).ToList(),
                Stats = stats,
                RecentActivities = recentActivities,
                FinancialSummary = financialSummary,
                QuickActions = quickActions,
                UpcomingEvents = new List<UpcomingEventDto>()
            };
        }

        public async Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId)
        {
            return await _dashboardRepo.GetStaffDashboardAsync(userId);
        }

        private static List<QuickActionDto> GetQuickActionsForRole(string role)
        {
            var actions = new List<QuickActionDto>();

            if (role == UserRole.SuperAdmin || role == UserRole.Manager)
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto
                    {
                        Title = "Onboard New Resident",
                        Icon = "bi-person-plus",
                        Url = "/Onboarding/Create",
                        Color = "success",
                        RequiresPermission = true
                    },
                    new QuickActionDto
                    {
                        Title = "Manage Community Members",
                        Icon = "bi-people",
                        Url = "/CommunityMembers/Index",
                        Color = "info",
                        RequiresPermission = true
                    },
                    new QuickActionDto
                    {
                        Title = "Manage Staff",
                        Icon = "bi-person-badge",
                        Url = "/StaffMembers/Index",
                        Color = "warning",
                        RequiresPermission = true
                    },
                    new QuickActionDto
                    {
                        Title = "View All Residents",
                        Icon = "bi-house-door",
                        Url = "/ResidentManagement/Index",
                        Color = "primary",
                        RequiresPermission = true
                    }
                });
            }

            if (role == UserRole.President ||
                role == UserRole.Secretary ||
                role == UserRole.Treasurer)
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto
                    {
                        Title = "Manage Staff",
                        Icon = "bi-person-badge",
                        Url = "/StaffMembers/Index",
                        Color = "warning",
                        RequiresPermission = true
                    },
                    new QuickActionDto
                    {
                        Title = "View Residents",
                        Icon = "bi-people",
                        Url = "/ResidentManagement/Index",
                        Color = "primary",
                        RequiresPermission = false
                    }
                });
            }

            return actions;
        }
    }
}
