using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for building role-specific dashboard data.
    ///
    /// Handles:
    /// - Admin dashboard (global stats + recent activity placeholder)
    /// - Owner dashboard (list of owned flats with tenant info)
    /// - Tenant dashboard (single flat summary)
    /// - Global dashboard stats (flat counts)
    /// </summary>
    public class DashboardService : IDashboardService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>
        /// Initialises DashboardService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        public DashboardService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        /// <summary>
        /// Builds the admin dashboard DTO for a given admin user.
        /// Includes the user's primary role, global stats, and a placeholder activity entry.
        /// </summary>
        /// <param name="userId">Unique identifier of the admin user.</param>
        /// <returns>Admin dashboard DTO.</returns>
        /// <exception cref="Exception">Thrown when user is not found.</exception>
        public async Task<AdminDashboardDto> GetAdminDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var roleName = user.UserRoles
                .Select(ur => ur.Role.Name)
                .FirstOrDefault() ?? "SuperAdmin";

            var stats = await GetDashboardStatsAsync();

            return new AdminDashboardDto
            {
                FullName = user.FullName,
                Role = roleName,
                Stats = stats,
                RecentActivities = new List<RecentActivityDto>
                {
                    new() { Activity = "System initialized", Timestamp = DateTime.UtcNow, Type = "System" }
                }
            };
        }

        /// <summary>
        /// Builds the owner dashboard DTO for a resident owner.
        /// Lists all flats the owner has a mapping for, including active tenant info.
        /// </summary>
        /// <param name="userId">Unique identifier of the owner user.</param>
        /// <returns>Owner dashboard DTO.</returns>
        public async Task<OwnerDashboardDto> GetOwnerDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId);
            var flats = await UoW.Flats.GetFlatsWithMappingsByOwnerIdAsync(userId);

            var myFlats = flats.Select(f =>
            {
                var activeTenant = f.UserFlatMappings?.FirstOrDefault(x => x.IsActive);
                return new FlatSummaryDto
                {
                    FlatId = f.Id,
                    FlatNumber = f.FlatNumber,
                    ApartmentName = f.Apartment?.Name ?? "N/A",
                    OwnerName = f.OwnerUser?.FullName ?? "",
                    TenantName = activeTenant?.User?.FullName ?? ""
                };
            }).ToList();

            return new OwnerDashboardDto
            {
                FullName = user?.FullName ?? "Owner",
                UserId = userId,
                MyFlats = myFlats,
                PendingComplaints = 0,
                PendingBills = 0,
                TotalOutstanding = 0
            };
        }

        /// <summary>
        /// Builds the tenant dashboard DTO for a tenant.
        /// Resolves the tenant's single active flat mapping and returns flat summary info.
        /// </summary>
        /// <param name="userId">Unique identifier of the tenant user.</param>
        /// <returns>Tenant dashboard DTO with flat details or null flat if not yet assigned.</returns>
        public async Task<TenantDashboardDto> GetTenantDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId);
            var mapping = (await UoW.UserFlatMappings.GetByUserIdAsync(userId))
                            .FirstOrDefault(x => x.IsActive);

            FlatSummaryDto? flatSummary = null;

            if (mapping?.Flat != null)
            {
                var flat = mapping.Flat;
                flatSummary = new FlatSummaryDto
                {
                    FlatId = flat.Id,
                    FlatNumber = flat.FlatNumber,
                    ApartmentName = flat.Apartment?.Name ?? "N/A",
                    OwnerName = flat.OwnerUser?.FullName ?? "",
                    TenantName = user?.FullName ?? ""
                };
            }

            return new TenantDashboardDto
            {
                FullName = user?.FullName ?? "Tenant",
                UserId = userId,
                MyFlat = flatSummary,
                PendingComplaints = 0,
                PendingRent = 0
            };
        }

        /// <summary>
        /// Returns high-level flat statistics: total, occupied, and vacant flat counts.
        /// Complaint, bill, and visitor counts are placeholders for future phases.
        /// </summary>
        /// <returns>Dashboard stats DTO.</returns>
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalFlats = await UoW.Flats.GetTotalCountAsync();
            var occupiedFlats = await UoW.Flats.GetOccupiedCountAsync();

            return new DashboardStatsDto
            {
                TotalResidents = 1,
                TotalFlats = totalFlats,
                OccupiedFlats = occupiedFlats,
                VacantFlats = totalFlats - occupiedFlats,
                PendingComplaints = 0,
                PendingBills = 0,
                TodaysVisitors = 0
            };
        }
    }
}

































/*using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork UoW;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var roleName = user.UserRoles
                .Select(ur => ur.Role.Name)
                .FirstOrDefault() ?? "SuperAdmin";

            var stats = await GetDashboardStatsAsync();

            return new AdminDashboardDto
            {
                FullName = user.FullName,
                Role = roleName,
                Stats = stats,
                RecentActivities = new List<RecentActivityDto>
                {
                    new() { Activity = "System initialized", Timestamp = DateTime.UtcNow, Type = "System" }
                }
            };
        }

        public async Task<OwnerDashboardDto> GetOwnerDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId);
            var flats = await UoW.Flats.GetFlatsWithMappingsByOwnerIdAsync(userId);

            var myFlats = flats.Select(f =>
            {
                var activeTenant = f.UserFlatMappings?.FirstOrDefault(x => x.IsActive);
                return new FlatSummaryDto
                {
                    FlatId = f.Id,
                    FlatNumber = f.FlatNumber,
                    ApartmentName = f.Apartment?.Name ?? "N/A",
                    OwnerName = f.OwnerUser?.FullName ?? "",
                    TenantName = activeTenant?.User?.FullName ?? ""
                };
            }).ToList();

            return new OwnerDashboardDto
            {
                FullName = user?.FullName ?? "Owner",
                UserId = userId,
                MyFlats = myFlats,
                PendingComplaints = 0,
                PendingBills = 0,
                TotalOutstanding = 0
            };
        }

        public async Task<TenantDashboardDto> GetTenantDashboardAsync(Guid userId)
        {
            var user = await UoW.Users.GetByIdAsync(userId);
            var mapping = (await UoW.UserFlatMappings.GetByUserIdAsync(userId))
                            .FirstOrDefault(x => x.IsActive);

            FlatSummaryDto? flatSummary = null;

            if (mapping?.Flat != null)
            {
                var flat = mapping.Flat;
                flatSummary = new FlatSummaryDto
                {
                    FlatId = flat.Id,
                    FlatNumber = flat.FlatNumber,
                    ApartmentName = flat.Apartment?.Name ?? "N/A",
                    OwnerName = flat.OwnerUser?.FullName ?? "",
                    TenantName = user?.FullName ?? ""
                };
            }

            return new TenantDashboardDto
            {
                FullName = user?.FullName ?? "Tenant",
                UserId = userId,
                MyFlat = flatSummary,
                PendingComplaints = 0,
                PendingRent = 0
            };
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalFlats = await UoW.Flats.GetTotalCountAsync();
            var occupiedFlats = await UoW.Flats.GetOccupiedCountAsync();

            return new DashboardStatsDto
            {
                TotalResidents = 1,
                TotalFlats = totalFlats,
                OccupiedFlats = occupiedFlats,
                VacantFlats = totalFlats - occupiedFlats,
                PendingComplaints = 0,
                PendingBills = 0,
                TodaysVisitors = 0
            };
        }
    }
}

*/






















/*using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository UserRepo;
        private readonly IFlatRepository FlatRepo;
        private readonly IApartmentRepository ApartmentRepo;
        private readonly IUserFlatMappingRepository UserFlatMappingRepo;

        public DashboardService(
            IUserRepository userRepository,
            IFlatRepository flatRepository,
            IApartmentRepository apartmentRepository,
            IUserFlatMappingRepository userFlatMappingRepository)
        {
            UserRepo = userRepository;
            FlatRepo = flatRepository;
            ApartmentRepo = apartmentRepository;
            UserFlatMappingRepo = userFlatMappingRepository;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync(Guid userId)
        {
            var user = await UserRepo.GetByIdAsync(userId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var roleName = user.UserRoles
                .Select(ur => ur.Role.Name)
                .FirstOrDefault() ?? "SuperAdmin";

            var stats = await GetDashboardStatsAsync();

            return new AdminDashboardDto
            {
                FullName = user.FullName,
                Role = roleName,
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
        }

        public async Task<OwnerDashboardDto> GetOwnerDashboardAsync(Guid userId)
        {
            var user = await UserRepo.GetByIdAsync(userId);

            var flats = await FlatRepo.GetFlatsWithMappingsByOwnerIdAsync(userId);

            var myFlats = flats.Select(f =>
            {
                var activeTenant = f.UserFlatMappings?
                    .FirstOrDefault(x => x.IsActive);

                return new FlatSummaryDto
                {
                    FlatId = f.Id,
                    FlatNumber = f.FlatNumber,
                    ApartmentName = f.Apartment?.Name ?? "N/A",
                    OwnerName = f.OwnerUser?.FullName ?? "",
                    TenantName = activeTenant?.User?.FullName ?? ""
                };
            }).ToList();

            return new OwnerDashboardDto
            {
                FullName = user?.FullName ?? "Owner",
                UserId = userId,
                MyFlats = myFlats,
                PendingComplaints = 0,
                PendingBills = 0,
                TotalOutstanding = 0
            };
        }

        public async Task<TenantDashboardDto> GetTenantDashboardAsync(Guid userId)
        {
            var user = await UserRepo.GetByIdAsync(userId);

            var mapping = (await UserFlatMappingRepo.GetByUserIdAsync(userId))
                            .FirstOrDefault(x => x.IsActive);

            FlatSummaryDto? flatSummary = null;

            if (mapping?.Flat != null)
            {
                var flat = mapping.Flat;

                flatSummary = new FlatSummaryDto
                {
                    FlatId = flat.Id,
                    FlatNumber = flat.FlatNumber,
                    ApartmentName = flat.Apartment?.Name ?? "N/A",
                    OwnerName = flat.OwnerUser?.FullName ?? "",
                    TenantName = user?.FullName ?? ""
                };
            }

            return new TenantDashboardDto
            {
                FullName = user?.FullName ?? "Tenant",
                UserId = userId,
                MyFlat = flatSummary,
                PendingComplaints = 0,
                PendingRent = 0
            };
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalFlats = await FlatRepo.GetTotalCountAsync();
            var occupiedFlats = await FlatRepo.GetOccupiedCountAsync();

            return new DashboardStatsDto
            {
                TotalResidents = 1,
                TotalFlats = totalFlats,
                OccupiedFlats = occupiedFlats,
                VacantFlats = totalFlats - occupiedFlats,
                PendingComplaints = 0,
                PendingBills = 0,
                TodaysVisitors = 0
            };
        }
    }
}


*/


















