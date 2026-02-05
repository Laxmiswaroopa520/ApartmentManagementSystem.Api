using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services;

public class EnhancedDashboardService : IEnhancedDashboardService
{
    private readonly IUserRepository UserRepo;
    private readonly IEnhancedDashboardRepository DashboardRepo;

    public EnhancedDashboardService(
        IUserRepository userRepo,
        IEnhancedDashboardRepository dashboardRepo)
    {
        UserRepo = userRepo;
        DashboardRepo = dashboardRepo;
    }

    public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
    {
        var user = await UserRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        // ⭐ FIX: .Where() does NOT change List<string?> to List<string>.
        // Chain .Select(r => r!) AFTER the .Where() to tell the compiler
        // "I know these are non-null because I just filtered nulls out."
        var roles = user.UserRoles?
            .Select(ur => ur.Role?.Name)
            .Where(r => !string.IsNullOrEmpty(r))
            .Select(r => r!)           // ← THIS converts List<string?> → List<string>
            .ToList() ?? new List<string>();

        var primaryRole = roles.FirstOrDefault() ?? "User";

        var stats = await DashboardRepo.GetAdvancedDashboardStatsAsync();

        if (roles.Contains(RoleNames.SuperAdmin))
        {
            await DashboardRepo.PopulateApartmentStatsAsync(stats);
        }

        FinancialSummaryDto? financialSummary = null;

        if (roles.Contains(RoleNames.SuperAdmin) || roles.Contains(RoleNames.Treasurer))
        {
            financialSummary = await DashboardRepo.GetFinancialSummaryAsync();
        }

        return new EnhancedAdminDashboardDto
        {
            FullName = user.FullName,
            Role = primaryRole,
            AllRoles = roles,
            Stats = stats,
            RecentActivities = await DashboardRepo.GetRecentActivitiesAsync(),
            FinancialSummary = financialSummary,
            QuickActions = await GetQuickActionsForRolesAsync(roles),
            UpcomingEvents = new List<UpcomingEventDto>()
        };
    }

    public async Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId)
    {
        return await DashboardRepo.GetStaffDashboardAsync(userId);
    }

    public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
    {
        return await DashboardRepo.GetAdvancedDashboardStatsAsync();
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
    {
        return await DashboardRepo.GetFinancialSummaryAsync();
    }

    // Implements the interface method (single role string)
    public async Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role)
    {
        var roles = new List<string> { role };
        return await GetQuickActionsForRolesAsync(roles);
    }

    // Private helper (multi-role list)
    private async Task<List<QuickActionDto>> GetQuickActionsForRolesAsync(List<string> roles)
    {
        var actions = new List<QuickActionDto>();

        if (roles.Any(r =>
            r == RoleNames.SuperAdmin ||
            r == RoleNames.Manager))
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
                }
            });
        }

        if (roles.Any(r =>
            r == RoleNames.President ||
            r == RoleNames.Secretary ||
            r == RoleNames.Treasurer))
        {
            actions.Add(new QuickActionDto
            {
                Title = "View Residents",
                Icon = "bi-people",
                Url = "/ResidentManagement/Index",
                Color = "primary",
                RequiresPermission = false
            });
        }

        return await Task.FromResult(actions);
    }
}


























































