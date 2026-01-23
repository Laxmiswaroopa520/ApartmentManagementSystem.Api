
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class EnhancedDashboardRepository : IEnhancedDashboardRepository
{
    private readonly AppDbContext DBContext;

    public EnhancedDashboardRepository(AppDbContext context)
    {
        DBContext = context;
    }

    public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
    {
        // Total residents (Owners + Tenants)
        var totalResidents = await DBContext.Users
            .CountAsync(u =>
                u.UserRoles.Any(ur =>
                    ur.Role.Name == RoleNames.ResidentOwner ||
                    ur.Role.Name == RoleNames.Tenant));

        var totalFlats = await DBContext.Flats.CountAsync();

        // Occupied flats = flats that have at least one resident
        var occupiedFlats = await DBContext.UserFlatMappings
            .Select(uf => uf.FlatId)
            .Distinct()
            .CountAsync();

        var vacantFlats = totalFlats - occupiedFlats;

        var pendingRegistrations = await DBContext.Users
            .CountAsync(u =>
                u.UserRoles.Any(ur =>
                    ur.Role.Name == RoleNames.ResidentOwner ||
                    ur.Role.Name == RoleNames.Tenant)
                && !u.UserFlatMappings.Any());

        var totalStaff = await DBContext.StaffMembers.CountAsync();
        var activeStaff = await DBContext.StaffMembers.CountAsync(s => s.IsActive);

        var communityRoles = RoleNames.GetCommunityRoles();

        var communityMembers = await DBContext.Users
            .CountAsync(u =>
                u.UserRoles.Any(ur => communityRoles.Contains(ur.Role.Name)));

        var activeSecurityPersonnel = await DBContext.StaffMembers
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
        var user = await DBContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        var staffMember = await DBContext.StaffMembers
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
