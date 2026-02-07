/*using ApartmentManagementSystem.Application.DTOs.Dashboard;
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








using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services;

public class EnhancedDashboardService : IEnhancedDashboardService
{
    private readonly IUserRepository UserRepo;
    private readonly IEnhancedDashboardRepository DashboardRepo;
    private readonly IApartmentRepository ApartmentRepo;
    private readonly IAdminResidentService AdminResidentService;

    public EnhancedDashboardService(
        IUserRepository userRepo,
        IEnhancedDashboardRepository dashboardRepo,
        IApartmentRepository apartmentRepo,
        IAdminResidentService adminResidentService)
    {
        UserRepo = userRepo;
        DashboardRepo = dashboardRepo;
        ApartmentRepo = apartmentRepo;
        AdminResidentService = adminResidentService;
    }

    public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
    {
        var user = await UserRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        var roles = user.UserRoles?
            .Select(ur => ur.Role?.Name)
            .Where(r => !string.IsNullOrEmpty(r))
            .Select(r => r!)
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

    public async Task<ManagerDashboardDto> GetManagerDashboardAsync(Guid userId)
    {
        var user = await UserRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        // Get manager's apartment assignment
        var managerAssignment = await ApartmentRepo.GetActiveManagerByUserIdAsync(userId)
            ?? throw new Exception("Manager is not assigned to any apartment");

        var apartment = managerAssignment.Apartment;

        // Get apartment-specific stats
        var stats = await DashboardRepo.GetApartmentDashboardStatsAsync(apartment.Id);

        // Get pending residents for this apartment
        var allPendingResidents = await AdminResidentService.GetPendingResidentsAsync();
        var apartmentPendingResidents = allPendingResidents; // TODO: Filter by apartment

        // Get notice board messages
        var noticeBoard = await DashboardRepo.GetNoticeBoardMessagesAsync(apartment.Id);

        return new ManagerDashboardDto
        {
            FullName = user.FullName,
            Role = "Manager",
            ApartmentId = apartment.Id,
            ApartmentName = apartment.Name,
            Stats = stats,
            RecentActivities = await DashboardRepo.GetApartmentRecentActivitiesAsync(apartment.Id),
            QuickActions = await GetQuickActionsForRoleAsync("Manager"),
            NoticeBoard = noticeBoard.Take(10).ToList(),
            PendingResidents = apartmentPendingResidents.Take(5).ToList()
        };
    }

    public async Task<CommunityLeaderDashboardDto> GetCommunityLeaderDashboardAsync(Guid userId, string role)
    {
        var user = await UserRepo.GetByIdAsync(userId)
            ?? throw new Exception("User not found");

        // Get community member's apartment
        var communityMember = await ApartmentRepo.DBContext.Set<CommunityMember>()
            .Include(cm => cm.Apartment)
            .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.IsActive)
            ?? throw new Exception($"{role} is not assigned to any apartment");

        var apartment = communityMember.Apartment;

        // Get user's flat number
        var flatMapping = user.UserFlatMappings?.FirstOrDefault(ufm => ufm.IsActive);
        var flatNumber = flatMapping?.Flat?.FlatNumber ?? "N/A";

        // Get apartment-specific stats
        var stats = await DashboardRepo.GetApartmentDashboardStatsAsync(apartment.Id);

        // Get notice board messages
        var noticeBoard = await DashboardRepo.GetNoticeBoardMessagesAsync(apartment.Id);

        FinancialSummaryDto? financialSummary = null;
        if (role == RoleNames.Treasurer)
        {
            financialSummary = await DashboardRepo.GetApartmentFinancialSummaryAsync(apartment.Id);
        }

        return new CommunityLeaderDashboardDto
        {
            FullName = user.FullName,
            Role = role,
            ApartmentId = apartment.Id,
            ApartmentName = apartment.Name,
            FlatNumber = flatNumber,
            Stats = stats,
            RecentActivities = await DashboardRepo.GetApartmentRecentActivitiesAsync(apartment.Id),
            QuickActions = await GetQuickActionsForRoleAsync(role),
            NoticeBoard = noticeBoard.Take(10).ToList(),
            FinancialSummary = financialSummary
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

    public async Task<ApartmentDashboardStatsDto> GetApartmentDashboardStatsAsync(Guid apartmentId)
    {
        return await DashboardRepo.GetApartmentDashboardStatsAsync(apartmentId);
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
    {
        return await DashboardRepo.GetFinancialSummaryAsync();
    }

    public async Task<FinancialSummaryDto> GetApartmentFinancialSummaryAsync(Guid apartmentId)
    {
        return await DashboardRepo.GetApartmentFinancialSummaryAsync(apartmentId);
    }

    public async Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role)
    {
        var roles = new List<string> { role };
        return await GetQuickActionsForRolesAsync(roles);
    }

    public async Task<List<NoticeBoardMessageDto>> GetNoticeBoardMessagesAsync(Guid apartmentId)
    {
        return await DashboardRepo.GetNoticeBoardMessagesAsync(apartmentId);
    }

    private async Task<List<QuickActionDto>> GetQuickActionsForRolesAsync(List<string> roles)
    {
        var actions = new List<QuickActionDto>();

        if (roles.Any(r => r == RoleNames.SuperAdmin))
        {
            actions.AddRange(new[]
            {
                new QuickActionDto
                {
                    Title = "Create Apartment",
                    Icon = "bi-building-add",
                    Url = "/ApartmentBuilder/Create",
                    Color = "primary",
                    RequiresPermission = true
                },
                new QuickActionDto
                {
                    Title = "My Apartments",
                    Icon = "bi-buildings",
                    Url = "/ApartmentBuilder/ManageApartments",
                    Color = "info",
                    RequiresPermission = true
                },
                new QuickActionDto
                {
                    Title = "Onboard Resident",
                    Icon = "bi-person-plus",
                    Url = "/Onboarding/Create",
                    Color = "success",
                    RequiresPermission = true
                },
                new QuickActionDto
                {
                    Title = "All Residents",
                    Icon = "bi-people",
                    Url = "/ResidentManagement/Index",
                    Color = "primary",
                    RequiresPermission = false
                }
            });
        }

        if (roles.Any(r => r == RoleNames.Manager))
        {
            actions.AddRange(new[]
            {
                new QuickActionDto
                {
                    Title = "Onboard Resident",
                    Icon = "bi-person-plus",
                    Url = "/Onboarding/Create",
                    Color = "success",
                    RequiresPermission = true
                },
                new QuickActionDto
                {
                    Title = "Community Members",
                    Icon = "bi-award",
                    Url = "/Community/Index",
                    Color = "info",
                    RequiresPermission = true
                },
                new QuickActionDto
                {
                    Title = "Staff Members",
                    Icon = "bi-person-badge",
                    Url = "/StaffMembers/Index",
                    Color = "warning",
                    RequiresPermission = true
                },
                new QuickActionDto
                {
                    Title = "Pending Assignments",
                    Icon = "bi-hourglass-split",
                    Url = "/AdminResidents/Pending",
                    Color = "danger",
                    RequiresPermission = true
                },
                new QuickActionDto
                {
                    Title = "Notice Board",
                    Icon = "bi-megaphone",
                    Url = "/NoticeBoard/Index",
                    Color = "primary",
                    RequiresPermission = false
                }
            });
        }

        if (roles.Any(r => r == RoleNames.President || r == RoleNames.Secretary))
        {
            actions.AddRange(new[]
            {
                new QuickActionDto
                {
                    Title = "Notice Board",
                    Icon = "bi-megaphone",
                    Url = "/NoticeBoard/Index",
                    Color = "primary",
                    RequiresPermission = false
                },
                new QuickActionDto
                {
                    Title = "View Residents",
                    Icon = "bi-people",
                    Url = "/ResidentManagement/Index",
                    Color = "info",
                    RequiresPermission = false
                },
                new QuickActionDto
                {
                    Title = "Staff Members",
                    Icon = "bi-person-badge",
                    Url = "/StaffMembers/Index",
                    Color = "warning",
                    RequiresPermission = false
                }
            });
        }

        if (roles.Any(r => r == RoleNames.Treasurer))
        {
            actions.AddRange(new[]
            {
                new QuickActionDto
                {
                    Title = "Financial Reports",
                    Icon = "bi-graph-up",
                    Url = "/Finance/Reports",
                    Color = "success",
                    RequiresPermission = false
                },
                new QuickActionDto
                {
                    Title = "View Residents",
                    Icon = "bi-people",
                    Url = "/ResidentManagement/Index",
                    Color = "info",
                    RequiresPermission = false
                }
            });
        }

        return await Task.FromResult(actions);
    }
}














































