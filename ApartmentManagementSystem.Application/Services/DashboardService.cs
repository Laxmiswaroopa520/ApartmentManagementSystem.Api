using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;


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

        /*     public async Task<AdminDashboardDto> GetAdminDashboardAsync(Guid userId)
             {
                 var user = await _userRepository.GetByIdAsync(userId);
                 var stats = await GetDashboardStatsAsync();

                 return new AdminDashboardDto
                 {
                     FullName = user?.FullName ?? "Admin",
                     Role = user?.Role.Name ?? "SuperAdmin",
                     Stats = stats,
                     RecentActivities = new List<RecentActivityDto>
                 {
                     new RecentActivityDto { Activity = "System initialized", Timestamp = DateTime.UtcNow, Type = "System" }
                 }
                 };
             }*/
        public async Task<AdminDashboardDto> GetAdminDashboardAsync(Guid userId)
        {
            var user = await UserRepo.GetByIdAsync(userId)
                ?? throw new Exception("User not found");

            var roleName = user.UserRoles
                .Select(ur => ur.Role.Name)
                .FirstOrDefault() ?? "SuperAdmin";

            var stats = await GetDashboardStatsAsync();

            return new AdminDashboardDto
            {
                FullName = user.FullName ?? "Admin",
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

        /*  public async Task<AdminDashboardDto> GetAdminDashboardAsync(Guid userId)
          {
              var user = await UserRepo.GetByIdAsync(userId);

              if (user == null)
                  throw new Exception("User not found");

              var stats = await GetDashboardStatsAsync();

              return new AdminDashboardDto
              {
                  FullName = user.FullName ?? "Admin",
                  Role = user.Role?.Name ?? "SuperAdmin",
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
        */

        /*     public async Task<OwnerDashboardDto> GetOwnerDashboardAsync(Guid userId)
             {
                 var user = await _userRepository.GetByIdAsync(userId);
                 var flats = await _flatRepository.GetByUserIdAsync(userId);
                 var mappings = await _userFlatMappingRepository.GetByUserIdAsync(userId);

                 var myFlats = flats.Select(f => new FlatSummaryDto
                 {
                     FlatId = f.Id,
                     FlatNumber = f.FlatNumber,
                     ApartmentName = f.Apartment.Name,
                     OwnerName = f.OwnerUser?.FullName ?? "",
                     TenantName = f.TenantUser?.FullName                 //changed from Owner to OwnerUser
                 }).ToList();

                 return new OwnerDashboardDto
                 {
                     FullName = user?.FullName ?? "Owner",
                     UserId = userId,
                     MyFlats = myFlats,
                     PendingComplaints = 0, // Phase 5
                     PendingBills = 0, // Phase 6
                     TotalOutstanding = 0 // Phase 6
                 };
             }
        */
        public async Task<OwnerDashboardDto> GetOwnerDashboardAsync(Guid userId)
        {
            var user = await UserRepo.GetByIdAsync(userId);

            var flats = await FlatRepo.GetFlatsWithMappingsByOwnerIdAsync(userId);

            var myFlats = flats.Select(f =>
            {
                var activeTenant = f.UserFlatMappings
                    .FirstOrDefault(x => x.IsActive);

                return new FlatSummaryDto
                {
                    FlatId = f.Id,
                    FlatNumber = f.FlatNumber,
                    ApartmentName = f.Apartment.Name,
                    OwnerName = f.OwnerUser?.FullName ?? "",
                    TenantName = activeTenant?.User.FullName ?? ""
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

        /*  public async Task<TenantDashboardDto> GetTenantDashboardAsync(Guid userId)
          {
              var user = await _userRepository.GetByIdAsync(userId);
              var flats = await _flatRepository.GetByUserIdAsync(userId);
              var flat = flats.FirstOrDefault();

              FlatSummaryDto? flatSummary = null;
              if (flat != null)
              {
                  flatSummary = new FlatSummaryDto
                  {
                      FlatId = flat.Id,
                      FlatNumber = flat.FlatNumber,
                      ApartmentName = flat.Apartment.Name,
                      OwnerName = flat.OwnerUser?.FullName ?? "",
                      TenantName = flat.TenantUser?.FullName
                  };
              }

              return new TenantDashboardDto
              {
                  FullName = user?.FullName ?? "Tenant",
                  UserId = userId,
                  MyFlat = flatSummary,
                  PendingComplaints = 0, // Phase 5
                  PendingRent = 0 // Phase 6
              };
          }
        */
        public async Task<TenantDashboardDto> GetTenantDashboardAsync(Guid userId)
        {
            var user = await UserRepo.GetByIdAsync(userId);

            var mapping = (await UserFlatMappingRepo.GetByUserIdAsync(userId))
                            .FirstOrDefault(x => x.IsActive);

            FlatSummaryDto? flatSummary = null;

            if (mapping != null)
            {
                var flat = mapping.Flat;

                flatSummary = new FlatSummaryDto
                {
                    FlatId = flat.Id,
                    FlatNumber = flat.FlatNumber,
                    ApartmentName = flat.Apartment.Name,
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
                TotalResidents = 1, // Will be calculated properly in Phase 3
                TotalFlats = totalFlats,
                OccupiedFlats = occupiedFlats,
                VacantFlats = totalFlats - occupiedFlats,
                PendingComplaints = 0, // Phase 5
                PendingBills = 0, // Phase 6
                TodaysVisitors = 0 // Phase 7
            };
        }
    }
}