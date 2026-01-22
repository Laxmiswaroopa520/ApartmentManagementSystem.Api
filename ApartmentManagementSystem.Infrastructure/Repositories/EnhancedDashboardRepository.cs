/*using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class EnhancedDashboardRepository : IEnhancedDashboardRepository
    {
        private readonly AppDbContext _context;

        public EnhancedDashboardRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
        {
            var totalResidents = await _context.Users
                .CountAsync(u => u.Roles.Any(r =>
                    r.Name == UserRole.ResidentOwner || r.Name == UserRole.Tenant));

            var totalFlats = await _context.Flats.CountAsync();
            var occupiedFlats = await _context.Flats
                .CountAsync(f => f.OwnerId != null || f.TenantId != null);

            var totalStaff = await _context.StaffMembers.CountAsync();
            var activeStaff = await _context.StaffMembers.CountAsync(s => s.IsActive);

            var communityMembers = await _context.Users
                .CountAsync(u => u.Roles.Any(r =>
                    UserRole.GetCommunityRoles().Contains(r.Name)));

            var activeSecurity = await _context.StaffMembers
                .CountAsync(s => s.StaffType == UserRole.Security && s.IsActive);

            return new AdvancedDashboardStatsDto
            {
                TotalResidents = totalResidents,
                TotalFlats = totalFlats,
                OccupiedFlats = occupiedFlats,
                VacantFlats = totalFlats - occupiedFlats,
                PendingRegistrations = totalResidents - occupiedFlats,
                TotalStaffMembers = totalStaff,
                ActiveStaffMembers = activeStaff,
                CommunityMembers = communityMembers,
                PendingComplaints = 0,
                ResolvedComplaintsThisMonth = 0,
                TotalOutstandingBills = 0,
                CollectionThisMonth = 0,
                TodaysVisitors = 0,
                ActiveSecurityPersonnel = activeSecurity
            };
        }
        
        public async Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId)
        {
            var staff = await _context.StaffMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId)
                ?? throw new Exception("Staff member not found");

            return new StaffDashboardDto
            {
                FullName = staff.User.FullName,
                StaffType = staff.StaffType,
                ShiftStart = DateTime.Today.AddHours(8),
                ShiftEnd = DateTime.Today.AddHours(17),
                TodaysTasks = 0,
                CompletedTasks = 0,
                PendingTasks = 0,
                MyTasks = new List<TaskDto>()
            };
        }

        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
        {
            var last6Months = Enumerable.Range(0, 6)
                .Select(i =>
                {
                    var m = DateTime.Now.AddMonths(-i);
                    return new MonthlyCollectionDto
                    {
                        Month = m.ToString("MMM yyyy"),
                        Amount = 0
                    };
                })
                .Reverse()
                .ToList();

            return new FinancialSummaryDto
            {
                TotalOutstanding = 0,
                CollectedThisMonth = 0,
                CollectedLastMonth = 0,
                PendingMaintenanceFees = 0,
                PendingUtilityBills = 0,
                Last6MonthsCollection = last6Months
            };
        }

        public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync()
        {
            return new List<RecentActivityDto>
            {
                new RecentActivityDto
                {
                    Activity = "New resident registered",
                    Type = "Registration",
                    Timestamp = DateTime.UtcNow.AddMinutes(-10)
                },
                new RecentActivityDto
                {
                    Activity = "Flat assigned to resident",
                    Type = "Assignment",
                    Timestamp = DateTime.UtcNow.AddMinutes(-25)
                }
            };
        }
    }
}
*/
// ApartmentManagementSystem.Infrastructure/Repositories/EnhancedDashboardRepository.cs
/*using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class EnhancedDashboardRepository : IEnhancedDashboardRepository
{
    private readonly AppDbContext _context;

    public EnhancedDashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
    {
        var ownerRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == RoleNames.ResidentOwner);
        var tenantRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == RoleNames.Tenant);

        var totalResidents = await _context.Users
            .CountAsync(u => u.RoleId == ownerRole.Id || u.RoleId == tenantRole.Id);

        var totalFlats = await _context.Flats.CountAsync();

        // Users have FlatId - count distinct flats that are occupied
        var occupiedFlats = await _context.Users
            .Where(u => u.FlatId != null &&
                       (u.RoleId == ownerRole.Id || u.RoleId == tenantRole.Id))
            .Select(u => u.FlatId)
            .Distinct()
            .CountAsync();

        var vacantFlats = totalFlats - occupiedFlats;

        var pendingRegistrations = await _context.Users
            .CountAsync(u => (u.RoleId == ownerRole.Id || u.RoleId == tenantRole.Id) &&
                            (!u.IsRegistrationCompleted || u.FlatId == null));

        var totalStaff = await _context.StaffMembers.CountAsync();
        var activeStaff = await _context.StaffMembers.CountAsync(s => s.IsActive);

        var communityRoles = RoleNames.GetCommunityRoles();
        var communityRoleIds = await _context.Roles
            .Where(r => communityRoles.Contains(r.Name))
            .Select(r => r.Id)
            .ToListAsync();

        var communityMembers = await _context.Users
            .CountAsync(u => communityRoleIds.Contains(u.RoleId));

        var activeSecurityPersonnel = await _context.StaffMembers
            .CountAsync(s => s.StaffType == RoleNames.Security && s.IsActive);

        return new AdvancedDashboardStatsDto
        {
            TotalResidents = totalResidents,
            TotalFlats = totalFlats,
            OccupiedFlats = occupiedFlats,
            VacantFlats = vacantFlats,
            PendingRegistrations = pendingRegistrations,
            TotalStaffMembers = totalStaff,
            ActiveStaffMembers = activeStaff,
            CommunityMembers = communityMembers,
            PendingComplaints = 0,
            ResolvedComplaintsThisMonth = 0,
            TotalOutstandingBills = 0m,
            CollectionThisMonth = 0m,
            TodaysVisitors = 0,
            ActiveSecurityPersonnel = activeSecurityPersonnel
        };
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
    {
        var last6Months = new List<MonthlyCollectionDto>();
        for (int i = 5; i >= 0; i--)
        {
            var month = DateTime.Now.AddMonths(-i);
            last6Months.Add(new MonthlyCollectionDto
            {
                Month = month.ToString("MMM yyyy"),
                Amount = 0m // TODO: Calculate from actual billing data
            });
        }

        return new FinancialSummaryDto
        {
            TotalOutstanding = 0m, // TODO: Sum from billing
            CollectedThisMonth = 0m,
            CollectedLastMonth = 0m,
            PendingMaintenanceFees = 0m,
            PendingUtilityBills = 0m,
            Last6MonthsCollection = last6Months
        };
    }

    public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync()
    {
        // TODO: Implement activity logging table and query it
        // For now, return sample data
        return new List<RecentActivityDto>
        {
            new RecentActivityDto
            {
                Activity = "New resident registered",
                Type = "Registration",
                Timestamp = DateTime.Now.AddMinutes(-10)
            },
            new RecentActivityDto
            {
                Activity = "Flat assigned to resident",
                Type = "Assignment",
                Timestamp = DateTime.Now.AddMinutes(-25)
            }
        };
    }

    public async Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        var staffMember = await _context.StaffMembers
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staffMember == null)
            throw new Exception("Staff member record not found");

        return new StaffDashboardDto
        {
            FullName = user.FullName,
            StaffType = staffMember.StaffType,
            ShiftStart = DateTime.Today.AddHours(8),
            ShiftEnd = DateTime.Today.AddHours(17),
            TodaysTasks = 0, // TODO: From tasks table
            CompletedTasks = 0,
            PendingTasks = 0,
            MyTasks = new List<TaskDto>()
        };
    }
}
*/
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class EnhancedDashboardRepository : IEnhancedDashboardRepository
{
    private readonly AppDbContext _context;

    public EnhancedDashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
    {
        // Total residents (Owners + Tenants)
        var totalResidents = await _context.Users
            .CountAsync(u =>
                u.UserRoles.Any(ur =>
                    ur.Role.Name == RoleNames.ResidentOwner ||
                    ur.Role.Name == RoleNames.Tenant));

        var totalFlats = await _context.Flats.CountAsync();

        // Occupied flats = flats that have at least one resident
        var occupiedFlats = await _context.UserFlatMappings
            .Select(uf => uf.FlatId)
            .Distinct()
            .CountAsync();

        var vacantFlats = totalFlats - occupiedFlats;

        var pendingRegistrations = await _context.Users
            .CountAsync(u =>
                u.UserRoles.Any(ur =>
                    ur.Role.Name == RoleNames.ResidentOwner ||
                    ur.Role.Name == RoleNames.Tenant)
                && !u.UserFlatMappings.Any());

        var totalStaff = await _context.StaffMembers.CountAsync();
        var activeStaff = await _context.StaffMembers.CountAsync(s => s.IsActive);

        var communityRoles = RoleNames.GetCommunityRoles();

        var communityMembers = await _context.Users
            .CountAsync(u =>
                u.UserRoles.Any(ur => communityRoles.Contains(ur.Role.Name)));

        var activeSecurityPersonnel = await _context.StaffMembers
            .CountAsync(s =>
                s.StaffType == RoleNames.Security &&
                s.IsActive);

        return new AdvancedDashboardStatsDto
        {
            TotalResidents = totalResidents,
            TotalFlats = totalFlats,
            OccupiedFlats = occupiedFlats,
            VacantFlats = vacantFlats,
            PendingRegistrations = pendingRegistrations,
            TotalStaffMembers = totalStaff,
            ActiveStaffMembers = activeStaff,
            CommunityMembers = communityMembers,
            PendingComplaints = 0,
            ResolvedComplaintsThisMonth = 0,
            TotalOutstandingBills = 0m,
            CollectionThisMonth = 0m,
            TodaysVisitors = 0,
            ActiveSecurityPersonnel = activeSecurityPersonnel
        };
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync()
    {
        var last6Months = new List<MonthlyCollectionDto>();

        for (int i = 5; i >= 0; i--)
        {
            var month = DateTime.UtcNow.AddMonths(-i);
            last6Months.Add(new MonthlyCollectionDto
            {
                Month = month.ToString("MMM yyyy"),
                Amount = 0m // TODO: real billing data
            });
        }

        return new FinancialSummaryDto
        {
            TotalOutstanding = 0m,
            CollectedThisMonth = 0m,
            CollectedLastMonth = 0m,
            PendingMaintenanceFees = 0m,
            PendingUtilityBills = 0m,
            Last6MonthsCollection = last6Months
        };
    }

    public async Task<List<RecentActivityDto>> GetRecentActivitiesAsync()
    {
        // TODO: replace with ActivityLog table
        return new List<RecentActivityDto>
        {
            new RecentActivityDto
            {
                Activity = "New resident registered",
                Type = "Registration",
                Timestamp = DateTime.UtcNow.AddMinutes(-10)
            },
            new RecentActivityDto
            {
                Activity = "Flat assigned to resident",
                Type = "Assignment",
                Timestamp = DateTime.UtcNow.AddMinutes(-25)
            }
        };
    }

    public async Task<StaffDashboardDto> GetStaffDashboardAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        var staffMember = await _context.StaffMembers
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (staffMember == null)
            throw new Exception("Staff member record not found");

        return new StaffDashboardDto
        {
            FullName = user.FullName,
            StaffType = staffMember.StaffType,
            ShiftStart = DateTime.Today.AddHours(8),
            ShiftEnd = DateTime.Today.AddHours(17),
            TodaysTasks = 0,
            CompletedTasks = 0,
            PendingTasks = 0,
            MyTasks = new List<TaskDto>()
        };
    }
}
