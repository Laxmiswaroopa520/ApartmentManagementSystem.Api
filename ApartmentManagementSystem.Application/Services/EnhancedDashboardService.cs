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








/*

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


*/













/* Correct one.....

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

        var roles = user.UserRoles?
            .Select(ur => ur.Role?.Name)
            .Where(r => !string.IsNullOrEmpty(r))
            .ToList() ?? new List<string>();

        var primaryRole = roles.FirstOrDefault() ?? "User";

        var stats = await DashboardRepo.GetAdvancedDashboardStatsAsync();

        //  Delegate data logic to repository
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

  /*  public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
    {
        var user = await UserRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        // Get all role names
        var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? string.Empty)
                                   .Where(r => !string.IsNullOrEmpty(r))
                                   .ToList() ?? new List<string>();

        var primaryRole = roles.FirstOrDefault() ?? "User";

        var stats = await DashboardRepo.GetAdvancedDashboardStatsAsync();
        var recentActivities = await DashboardRepo.GetRecentActivitiesAsync();
        var quickActions = await GetQuickActionsForRolesAsync(roles);

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
            RecentActivities = recentActivities,
            FinancialSummary = financialSummary,
            QuickActions = quickActions,
            UpcomingEvents = new List<UpcomingEventDto>()
        };
    }
  */
/*
    public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var primaryRole = roles.FirstOrDefault();

        var stats = await _dashboardRepo.GetAdvancedDashboardStatsAsync();
        var recentActivities = await _dashboardRepo.GetRecentActivitiesAsync();
        var quickActions = await GetQuickActionsForRolesAsync(roles);

        FinancialSummaryDto? financialSummary = null;

        if (roles.Contains(RoleNames.SuperAdmin) ||
            roles.Contains(RoleNames.Treasurer))
        {
            financialSummary = await _dashboardRepo.GetFinancialSummaryAsync();
        }

        return new EnhancedAdminDashboardDto
        {
            FullName = user.FullName,
            Role = primaryRole,
            AllRoles = roles,
            Stats = stats,
            RecentActivities = recentActivities,
            FinancialSummary = financialSummary,
            QuickActions = quickActions,
            UpcomingEvents = new List<UpcomingEventDto>()
        };
    }
    ---------
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

// THIS NOW IMPLEMENTS THE INTERFACE
public async Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role)
{
    var roles = new List<string> { role };
    return await GetQuickActionsForRolesAsync(roles);
}
//private helper
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
// Application/Services/EnhancedDashboardService.cs - Add apartment stats method

/* public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
{
    var user = await UserRepo.GetByIdAsync(userId)
        ?? throw new Exception("User not found");

    var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
    var stats = await GetAdvancedDashboardStatsAsync();

    // ⭐ Populate apartment stats for SuperAdmin
    if (roles.Contains("SuperAdmin"))
    {
        stats.TotalApartments = await ApartmentRepo.GetTotalCountAsync();

        var allApartments = await ApartmentRepo.GetAllAsync();
        stats.ActiveApartments = allApartments.Count(a => a.Status == ApartmentStatus.Active);
        stats.ApartmentsUnderConstruction = allApartments.Count(a => a.Status == ApartmentStatus.UnderConstruction);

        var apartmentsWithDetails = await ApartmentRepo.GetAllWithDetailsAsync();
        stats.TotalFloors = apartmentsWithDetails.Sum(a => a.TotalFloors);

        stats.TotalManagers = await _context.Set<ApartmentManager>()
            .CountAsync(m => m.IsActive);
    }

    return new EnhancedAdminDashboardDto
    {
        FullName = user.FullName,
        AllRoles = roles,
        Stats = stats,
        RecentActivities = new List<RecentActivityDto>
    {
        new RecentActivityDto
        {
            Activity = "System initialized",
            Timestamp = DateTime.UtcNow,
            Type = "System"
        }
    }
    };
}--------------------
}

*/













































