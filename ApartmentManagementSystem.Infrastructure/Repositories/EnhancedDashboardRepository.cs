using ApartmentManagementSystem.Application.DTOs.Dashboard;
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
        /*
        public async Task<AdvancedDashboardStatsDto> GetAdvancedDashboardStatsAsync()
        {
            var totalFlats = await _context.Flats.CountAsync();

            var occupiedFlats = await _context.Flats
                .CountAsync(f => f.OwnerId != null || f.TenantId != null);

            var totalResidents = await _context.Flats
                .CountAsync(f => f.OwnerId != null) +
                await _context.Flats.CountAsync(f => f.TenantId != null);

            var totalStaff = await _context.StaffMembers.CountAsync();
            var activeStaff = await _context.StaffMembers.CountAsync(s => s.IsActive);

            var activeSecurity = await _context.StaffMembers
                .CountAsync(s =>
                    s.StaffType == UserRole.Security &&
                    s.IsActive);

            return new AdvancedDashboardStatsDto
            {
                TotalResidents = totalResidents,
                TotalFlats = totalFlats,
                OccupiedFlats = occupiedFlats,
                VacantFlats = totalFlats - occupiedFlats,

                // REAL meaning: flats waiting for allocation
                PendingRegistrations = totalFlats - occupiedFlats,

                TotalStaffMembers = totalStaff,
                ActiveStaffMembers = activeStaff,
                ActiveSecurityPersonnel = activeSecurity,

                // Placeholder (future modules)
                PendingComplaints = 0,
                ResolvedComplaintsThisMonth = 0,
                TotalOutstandingBills = 0,
                CollectionThisMonth = 0,
                TodaysVisitors = 0,
                CommunityMembers = 0
            };
        }
        */
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
