using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for building enhanced, role-aware dashboard data.
    ///
    /// Handles dashboards for:
    /// - SuperAdmin / Treasurer: global stats, apartment portfolio, financial summary
    /// - Manager: apartment-scoped stats, pending residents, notice board
    /// - Community Leaders (President, Secretary, Treasurer): apartment stats, notice board
    /// - Staff: shift info and task placeholder
    ///
    /// Quick actions are generated dynamically based on the user's roles.
    /// </summary>
    public class EnhancedDashboardService : IEnhancedDashboardService
    {
        /// <summary>Unit of Work providing access to all repositories and dashboard repo.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>Admin resident service used to fetch pending residents for manager dashboard.</summary>
        private readonly IAdminResidentService AdminResidentService;

        /// <summary>
        /// Initialises EnhancedDashboardService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        /// <param name="adminResidentService">Service for fetching pending residents.</param>
        public EnhancedDashboardService(
            IUnitOfWork unitOfWork,
            IAdminResidentService adminResidentService)
        {
            UoW = unitOfWork;
            AdminResidentService = adminResidentService;
        }

        /// <summary>
        /// Builds the enhanced admin dashboard for a SuperAdmin or Treasurer.
        ///
        /// - SuperAdmin: receives apartment portfolio stats in addition to global stats.
        /// - Treasurer (or SuperAdmin): receives financial summary.
        /// - All: receive recent activities and role-specific quick actions.
        /// </summary>
        /// <param name="userId">Unique identifier of the admin/treasurer user.</param>
        /// <returns>Enhanced admin dashboard DTO.</returns>
        /// <exception cref="Exception">Thrown when user is not found.</exception>
        public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var roles = user.UserRoles?
                .Select(ur => ur.Role?.Name)
                .Where(r => !string.IsNullOrEmpty(r))
                .Select(r => r!)
                .ToList() ?? new List<string>();

            var primaryRole = roles.FirstOrDefault() ?? "User";
            var stats = await UoW.Dashboard.GetAdvancedDashboardStatsAsync();

            if (roles.Contains(RoleNames.SuperAdmin))
                await UoW.Dashboard.PopulateApartmentStatsAsync(stats);

            FinancialSummaryDto? financialSummary = null;
            if (roles.Contains(RoleNames.SuperAdmin) || roles.Contains(RoleNames.Treasurer))
                financialSummary = await UoW.Dashboard.GetFinancialSummaryAsync();

            return new EnhancedAdminDashboardDto
            {
                FullName = user.FullName,
                Role = primaryRole,
                AllRoles = roles,
                Stats = stats,
                RecentActivities = await UoW.Dashboard.GetRecentActivitiesAsync(),
                FinancialSummary = financialSummary,
                QuickActions = await GetQuickActionsForRolesAsync(roles),
                UpcomingEvents = new List<UpcomingEventDto>()
            };
        }

        /// <summary>
        /// Builds the manager dashboard scoped to the manager's assigned apartment.
        ///
        /// Includes apartment-specific stats, up to 5 pending residents, and the notice board.
        /// </summary>
        /// <param name="userId">Unique identifier of the manager user.</param>
        /// <returns>Manager dashboard DTO.</returns>
        /// <exception cref="Exception">Thrown when user not found or manager has no apartment assignment.</exception>
        public async Task<ManagerDashboardDto> GetManagerDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var managerAssignment = await UoW.Apartments.GetActiveManagerByUserIdAsync(userId)
                ?? throw new Exception("Manager is not assigned to any apartment");

            var apartment = managerAssignment.Apartment
                ?? throw new Exception("Apartment data not found");

            var stats = await UoW.Dashboard.GetApartmentDashboardStatsAsync(apartment.Id);
            var pendingResidents = await AdminResidentService.GetPendingResidentsAsync();
            var noticeBoard = await UoW.Dashboard.GetNoticeBoardMessagesAsync(apartment.Id);

            return new ManagerDashboardDto
            {
                FullName = user.FullName,
                Role = "Manager",
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                Stats = stats,
                RecentActivities = await UoW.Dashboard.GetApartmentRecentActivitiesAsync(apartment.Id),
                QuickActions = await GetQuickActionsForRoleAsync("Manager"),
                NoticeBoard = noticeBoard.Take(10).ToList(),
                PendingResidents = pendingResidents.Take(5).ToList()
            };
        }

        /// <summary>
        /// Builds the community leader dashboard scoped to the leader's assigned apartment.
        ///
        /// Treasurer additionally receives the apartment financial summary.
        /// </summary>
        /// <param name="userId">Unique identifier of the community leader user.</param>
        /// <param name="role">The specific community role (President, Secretary, or Treasurer).</param>
        /// <returns>Community leader dashboard DTO.</returns>
        /// <exception cref="Exception">Thrown when user not found or not assigned to an apartment.</exception>
        public async Task<CommunityLeaderDashboardDto> GetCommunityLeaderDashboardAsync(
            Guid userId, string role)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var communityMember = await UoW.CommunityMembers.GetByUserIdAsync(userId)
                ?? throw new Exception($"{role} is not assigned to any apartment");

            var apartment = communityMember.Apartment
                ?? throw new Exception(ErrorMessages.ApartmentDataNotFound);

            var flatNumber = user.UserFlatMappings?
                .FirstOrDefault(ufm => ufm.IsActive)?.Flat?.FlatNumber ?? "N/A";

            var stats = await UoW.Dashboard.GetApartmentDashboardStatsAsync(apartment.Id);
            var noticeBoard = await UoW.Dashboard.GetNoticeBoardMessagesAsync(apartment.Id);

            FinancialSummaryDto? financialSummary = null;
            if (role == RoleNames.Treasurer)
                financialSummary = await UoW.Dashboard.GetApartmentFinancialSummaryAsync(apartment.Id);

            return new CommunityLeaderDashboardDto
            {
                FullName = user.FullName,
                Role = role,
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                FlatNumber = flatNumber,
                Stats = stats,
                RecentActivities = await UoW.Dashboard.GetApartmentRecentActivitiesAsync(apartment.Id),
                QuickActions = await GetQuickActionsForRoleAsync(role),
                NoticeBoard = noticeBoard.Take(10).ToList(),
                FinancialSummary = financialSummary
            };
        }

        /// <summary>
        /// Retrieves the staff dashboard for a staff member user.
        /// Contains shift times and task placeholders (Phase 5).
        /// </summary>
        /// <param name="userId">Unique identifier of the staff user.</param>
        /// <returns>Staff dashboard DTO.</returns>
        public async Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId)
            => await UoW.Dashboard.GetStaffDashboardAsync(userId);

        /// <summary>Returns global advanced dashboard statistics.</summary>
        public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
            => await UoW.Dashboard.GetAdvancedDashboardStatsAsync();

        /// <summary>Returns apartment-scoped dashboard statistics.</summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        public async Task<ApartmentDashboardStatsDto> GetApartmentDashboardStatsAsync(Guid apartmentId)
            => await UoW.Dashboard.GetApartmentDashboardStatsAsync(apartmentId);

        /// <summary>Returns the global financial summary (Phase 6 placeholder).</summary>
        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
            => await UoW.Dashboard.GetFinancialSummaryAsync();

        /// <summary>Returns the apartment-scoped financial summary (Phase 6 placeholder).</summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        public async Task<FinancialSummaryDto> GetApartmentFinancialSummaryAsync(Guid apartmentId)
            => await UoW.Dashboard.GetApartmentFinancialSummaryAsync(apartmentId);

        /// <summary>
        /// Returns quick action tiles appropriate for a single role.
        /// Delegates to the multi-role helper.
        /// </summary>
        /// <param name="role">Role name string.</param>
        /// <returns>List of quick action DTOs.</returns>
        public async Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role)
            => await GetQuickActionsForRolesAsync(new List<string> { role });

        /// <summary>Returns notice board messages for a specific apartment (Phase 5 placeholder).</summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        public async Task<List<NoticeBoardMessageDto>> GetNoticeBoardMessagesAsync(Guid apartmentId)
            => await UoW.Dashboard.GetNoticeBoardMessagesAsync(apartmentId);

        // ── Private Helpers ────────────────────────────────────────

        /// <summary>
        /// Builds the list of quick action tiles for a given set of roles.
        /// Each role contributes its own set of actions; duplicates may exist
        /// if a user holds multiple roles.
        /// </summary>
        /// <param name="roles">List of role names held by the current user.</param>
        /// <returns>List of quick action DTOs.</returns>
        private async Task<List<QuickActionDto>> GetQuickActionsForRolesAsync(List<string> roles)
        {
            var actions = new List<QuickActionDto>();

            if (roles.Contains(RoleNames.SuperAdmin))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Create Apartment",  Icon = "bi-building-add", Url = "/ApartmentBuilder/Create",           Color = "primary", RequiresPermission = true  },
                    new QuickActionDto { Title = "My Apartments",     Icon = "bi-buildings",    Url = "/ApartmentBuilder/ManageApartments", Color = "info",    RequiresPermission = true  },
                    new QuickActionDto { Title = "Onboard Resident",  Icon = "bi-person-plus",  Url = "/Onboarding/Create",                 Color = "success", RequiresPermission = true  },
                    new QuickActionDto { Title = "All Residents",     Icon = "bi-people",       Url = "/ResidentManagement/Index",          Color = "primary", RequiresPermission = false }
                });
            }

            if (roles.Contains(RoleNames.Manager))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Onboard Resident",    Icon = "bi-person-plus",     Url = "/Onboarding/Create",      Color = "success", RequiresPermission = true  },
                    new QuickActionDto { Title = "Community Members",   Icon = "bi-award",           Url = "/Community/Index",        Color = "info",    RequiresPermission = true  },
                    new QuickActionDto { Title = "Staff Members",       Icon = "bi-person-badge",    Url = "/StaffMembers/Index",     Color = "warning", RequiresPermission = true  },
                    new QuickActionDto { Title = "Pending Assignments", Icon = "bi-hourglass-split", Url = "/AdminResidents/Pending", Color = "danger",  RequiresPermission = true  },
                    new QuickActionDto { Title = "Notice Board",        Icon = "bi-megaphone",       Url = "/NoticeBoard/Index",      Color = "primary", RequiresPermission = false }
                });
            }

            if (roles.Any(r => r == RoleNames.President || r == RoleNames.Secretary))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Notice Board",   Icon = "bi-megaphone",    Url = "/NoticeBoard/Index",        Color = "primary", RequiresPermission = false },
                    new QuickActionDto { Title = "View Residents", Icon = "bi-people",       Url = "/ResidentManagement/Index", Color = "info",    RequiresPermission = false },
                    new QuickActionDto { Title = "Staff Members",  Icon = "bi-person-badge", Url = "/StaffMembers/Index",       Color = "warning", RequiresPermission = false }
                });
            }

            if (roles.Contains(RoleNames.Treasurer))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Financial Reports", Icon = "bi-graph-up", Url = "/Finance/Reports",          Color = "success", RequiresPermission = false },
                    new QuickActionDto { Title = "View Residents",    Icon = "bi-people",   Url = "/ResidentManagement/Index", Color = "info",    RequiresPermission = false }
                });
            }

            return await Task.FromResult(actions);
        }
    }
}























































/*using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class EnhancedDashboardService : IEnhancedDashboardService
    {
        private readonly IUnitOfWork UoW;
        private readonly IAdminResidentService AdminResidentService;

        public EnhancedDashboardService(
            IUnitOfWork unitOfWork,
            IAdminResidentService adminResidentService)
        {
            UoW = unitOfWork;
            AdminResidentService = adminResidentService;
        }

        public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var roles = user.UserRoles?
                .Select(ur => ur.Role?.Name)
                .Where(r => !string.IsNullOrEmpty(r))
                .Select(r => r!)
                .ToList() ?? new List<string>();

            var primaryRole = roles.FirstOrDefault() ?? "User";
            var stats = await UoW.Dashboard.GetAdvancedDashboardStatsAsync();

            if (roles.Contains(RoleNames.SuperAdmin))
                await UoW.Dashboard.PopulateApartmentStatsAsync(stats);

            FinancialSummaryDto? financialSummary = null;
            if (roles.Contains(RoleNames.SuperAdmin) || roles.Contains(RoleNames.Treasurer))
                financialSummary = await UoW.Dashboard.GetFinancialSummaryAsync();

            return new EnhancedAdminDashboardDto
            {
                FullName = user.FullName,
                Role = primaryRole,
                AllRoles = roles,
                Stats = stats,
                RecentActivities = await UoW.Dashboard.GetRecentActivitiesAsync(),
                FinancialSummary = financialSummary,
                QuickActions = await GetQuickActionsForRolesAsync(roles),
                UpcomingEvents = new List<UpcomingEventDto>()
            };
        }

        public async Task<ManagerDashboardDto> GetManagerDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var managerAssignment = await UoW.Apartments.GetActiveManagerByUserIdAsync(userId)
                ?? throw new Exception("Manager is not assigned to any apartment");

            var apartment = managerAssignment.Apartment
                ?? throw new Exception("Apartment data not found");

            var stats = await UoW.Dashboard.GetApartmentDashboardStatsAsync(apartment.Id);
            var pendingResidents = await AdminResidentService.GetPendingResidentsAsync();
            var noticeBoard = await UoW.Dashboard.GetNoticeBoardMessagesAsync(apartment.Id);

            return new ManagerDashboardDto
            {
                FullName = user.FullName,
                Role = "Manager",
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                Stats = stats,
                RecentActivities = await UoW.Dashboard.GetApartmentRecentActivitiesAsync(apartment.Id),
                QuickActions = await GetQuickActionsForRoleAsync("Manager"),
                NoticeBoard = noticeBoard.Take(10).ToList(),
                PendingResidents = pendingResidents.Take(5).ToList()
            };
        }

        public async Task<CommunityLeaderDashboardDto> GetCommunityLeaderDashboardAsync(
            Guid userId, string role)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var communityMember = await UoW.CommunityMembers.GetByUserIdAsync(userId)
                ?? throw new Exception($"{role} is not assigned to any apartment");

            var apartment = communityMember.Apartment
                ?? throw new Exception(ErrorMessages.ApartmentDataNotFound);

            var flatNumber = user.UserFlatMappings?
                .FirstOrDefault(ufm => ufm.IsActive)?.Flat?.FlatNumber ?? "N/A";

            var stats = await UoW.Dashboard.GetApartmentDashboardStatsAsync(apartment.Id);
            var noticeBoard = await UoW.Dashboard.GetNoticeBoardMessagesAsync(apartment.Id);

            FinancialSummaryDto? financialSummary = null;
            if (role == RoleNames.Treasurer)
                financialSummary = await UoW.Dashboard.GetApartmentFinancialSummaryAsync(apartment.Id);

            return new CommunityLeaderDashboardDto
            {
                FullName = user.FullName,
                Role = role,
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                FlatNumber = flatNumber,
                Stats = stats,
                RecentActivities = await UoW.Dashboard.GetApartmentRecentActivitiesAsync(apartment.Id),
                QuickActions = await GetQuickActionsForRoleAsync(role),
                NoticeBoard = noticeBoard.Take(10).ToList(),
                FinancialSummary = financialSummary
            };
        }

        public async Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId)
            => await UoW.Dashboard.GetStaffDashboardAsync(userId);

        public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
            => await UoW.Dashboard.GetAdvancedDashboardStatsAsync();

        public async Task<ApartmentDashboardStatsDto> GetApartmentDashboardStatsAsync(Guid apartmentId)
            => await UoW.Dashboard.GetApartmentDashboardStatsAsync(apartmentId);

        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
            => await UoW.Dashboard.GetFinancialSummaryAsync();

        public async Task<FinancialSummaryDto> GetApartmentFinancialSummaryAsync(Guid apartmentId)
            => await UoW.Dashboard.GetApartmentFinancialSummaryAsync(apartmentId);

        public async Task<List<QuickActionDto>> GetQuickActionsForRoleAsync(string role)
            => await GetQuickActionsForRolesAsync(new List<string> { role });

        public async Task<List<NoticeBoardMessageDto>> GetNoticeBoardMessagesAsync(Guid apartmentId)
            => await UoW.Dashboard.GetNoticeBoardMessagesAsync(apartmentId);

        private async Task<List<QuickActionDto>> GetQuickActionsForRolesAsync(List<string> roles)
        {
            var actions = new List<QuickActionDto>();

            if (roles.Contains(RoleNames.SuperAdmin))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Create Apartment",  Icon = "bi-building-add", Url = "/ApartmentBuilder/Create",           Color = "primary", RequiresPermission = true  },
                    new QuickActionDto { Title = "My Apartments",     Icon = "bi-buildings",    Url = "/ApartmentBuilder/ManageApartments", Color = "info",    RequiresPermission = true  },
                    new QuickActionDto { Title = "Onboard Resident",  Icon = "bi-person-plus",  Url = "/Onboarding/Create",                 Color = "success", RequiresPermission = true  },
                    new QuickActionDto { Title = "All Residents",     Icon = "bi-people",       Url = "/ResidentManagement/Index",          Color = "primary", RequiresPermission = false }
                });
            }

            if (roles.Contains(RoleNames.Manager))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Onboard Resident",     Icon = "bi-person-plus",    Url = "/Onboarding/Create",       Color = "success", RequiresPermission = true  },
                    new QuickActionDto { Title = "Community Members",    Icon = "bi-award",          Url = "/Community/Index",         Color = "info",    RequiresPermission = true  },
                    new QuickActionDto { Title = "Staff Members",        Icon = "bi-person-badge",   Url = "/StaffMembers/Index",      Color = "warning", RequiresPermission = true  },
                    new QuickActionDto { Title = "Pending Assignments",  Icon = "bi-hourglass-split",Url = "/AdminResidents/Pending",  Color = "danger",  RequiresPermission = true  },
                    new QuickActionDto { Title = "Notice Board",         Icon = "bi-megaphone",      Url = "/NoticeBoard/Index",       Color = "primary", RequiresPermission = false }
                });
            }

            if (roles.Any(r => r == RoleNames.President || r == RoleNames.Secretary))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Notice Board",  
                        Icon = "bi-megaphone", 
                        Url = "/NoticeBoard/Index",    
                        Color = "primary", 
                        RequiresPermission = false },
                    new QuickActionDto { Title = "View Residents", 
                        Icon = "bi-people",    
                        Url = "/ResidentManagement/Index",
                        Color = "info",  
                        RequiresPermission = false },
                    new QuickActionDto { Title = "Staff Members",  
                        Icon = "bi-person-badge",
                        Url = "/StaffMembers/Index",     
                        Color = "warning",
                        RequiresPermission = false }
                });

            }

            if (roles.Contains(RoleNames.Treasurer))
            {
                actions.AddRange(new[]
                {
                    new QuickActionDto { Title = "Financial Reports",
                        Icon = "bi-graph-up", Url = "/Finance/Reports", 
                        Color = "success",
                        RequiresPermission = false },
                    new QuickActionDto {
                        Title = "View Residents",   
                        Icon = "bi-people",
                        Url = "/ResidentManagement/Index", 
                        Color = "info", 
                        RequiresPermission = false }
                });
            }

            return await Task.FromResult(actions);
        }
    }
}
*/






















/*
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services;

public class EnhancedDashboardService : IEnhancedDashboardService
{
    private readonly IUserRepository UserRepo;
    private readonly IEnhancedDashboardRepository DashboardRepo;
    private readonly IApartmentRepository ApartmentRepo;
    private readonly IAdminResidentService AdminResidentService;
    private readonly ICommunityMemberRepository CommunityMemberRepo;

    public EnhancedDashboardService(
        IUserRepository userRepo,
        IEnhancedDashboardRepository dashboardRepo,
        IApartmentRepository apartmentRepo,
        IAdminResidentService adminResidentService,
        ICommunityMemberRepository communityMemberRepo)
    {
        UserRepo = userRepo;
        DashboardRepo = dashboardRepo;
        ApartmentRepo = apartmentRepo;
        AdminResidentService = adminResidentService;
        CommunityMemberRepo = communityMemberRepo;
    }

    public async Task<EnhancedAdminDashboardDto> GetEnhancedAdminDashboardAsync(Guid userId)
    {
        var user = await UserRepo.GetByIdAsync(userId)
            ?? throw new Exception(ErrorMessages.UserNotFound);

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
            ?? throw new Exception(ErrorMessages.UserNotFound);

        // Get manager's apartment assignment
        var managerAssignment = await ApartmentRepo.GetActiveManagerByUserIdAsync(userId)
            ?? throw new Exception("Manager is not assigned to any apartment");

        var apartment = managerAssignment.Apartment
            ?? throw new Exception("Apartment data not found");

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
            ?? throw new Exception(ErrorMessages.UserNotFound);

        // Use ICommunityMemberRepository instead of DBContext
        var communityMember = await CommunityMemberRepo.GetByUserIdAsync(userId)
            ?? throw new Exception($"{role} is not assigned to any apartment");

        var apartment = communityMember.Apartment
            ?? throw new Exception(ErrorMessages.ApartmentDataNotFound);

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
*/











































