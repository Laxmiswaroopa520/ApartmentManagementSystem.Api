// Application/Services/AdminResidentService.cs
using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class AdminResidentService : IAdminResidentService
    {
        private readonly IUserRepository UserRepo;
        private readonly IFlatRepository FatRepo;
        private readonly IFloorRepository FloorRepo;
        private readonly IApartmentRepository ApartmentRepo;
        private readonly IUserFlatMappingRepository UserFlatMappingRepo;
        private readonly IEmailService EmailService;

        public AdminResidentService(
            IUserRepository userRepository,
            IFlatRepository flatRepository,
            IFloorRepository floorRepository,
            IApartmentRepository apartmentRepository,
            IUserFlatMappingRepository userFlatMappingRepository,
            IEmailService emailService)
        {
            UserRepo = userRepository;
            FatRepo = flatRepository;
            FloorRepo = floorRepository;
            ApartmentRepo = apartmentRepository;
            UserFlatMappingRepo = userFlatMappingRepository;
            EmailService = emailService;
        }

        public async Task<List<PendingResidentDto>> GetPendingResidentsAsync()
        {
            var users = await UserRepo.GetPendingResidentsAsync();
            return users.Select(u => new PendingResidentDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                PrimaryPhone = u.PrimaryPhone,
                Email = u.Email ?? "",
                ResidentType = u.ResidentType?.ToString() ?? "Unknown",
                RegisteredOn = u.CreatedAt,
                Status = u.Status.ToString()
            }).ToList();
        }

        // ⭐ NEW: Get apartments based on user role
        public async Task<List<ApartmentDropdownDto>> GetApartmentsForUserAsync(Guid userId, string role)
        {
            if (role == "SuperAdmin")
            {
                // SuperAdmin sees ALL apartments
                var allApartments = await ApartmentRepo.GetAllAsync();
                return allApartments
                    .Where(a => a.IsActive)
                    .Select(a => new ApartmentDropdownDto
                    {
                        Id = a.Id,
                        Name = a.Name
                    })
                    .OrderBy(a => a.Name)
                    .ToList();
            }
            else if (role == "Manager")
            {
                // Manager sees only their assigned apartment
                var manager = await ApartmentRepo.GetActiveManagerByUserIdAsync(userId);
                if (manager != null)
                {
                    var apartment = await ApartmentRepo.GetByIdAsync(manager.ApartmentId);
                    if (apartment != null && apartment.IsActive)
                    {
                        return new List<ApartmentDropdownDto>
                        {
                            new ApartmentDropdownDto
                            {
                                Id = apartment.Id,
                                Name = apartment.Name
                            }
                        };
                    }
                }
            }

            return new List<ApartmentDropdownDto>();
        }

        // ⭐ NEW: Get floors by apartment
        public async Task<List<FloorDto>> GetFloorsByApartmentAsync(Guid apartmentId)
        {
            var floors = await FloorRepo.GetByApartmentIdAsync(apartmentId);
            return floors
                .OrderBy(f => f.FloorNumber)
                .Select(f => new FloorDto
                {
                    Id = f.Id,
                    FloorNumber = f.FloorNumber,
                    ApartmentId = f.ApartmentId,
                    ApartmentName = f.Apartment?.Name ?? ""
                })
                .ToList();
        }

        public async Task<List<FlatDto>> GetVacantFlatsByFloorAsync(Guid floorId)
        {
            var flats = await FatRepo.GetVacantFlatsByFloorAsync(floorId);
            return flats
                .OrderBy(f => f.FlatNumber)
                .Select(f => new FlatDto
                {
                    Id = f.Id,
                    FlatNumber = f.FlatNumber,
                    FloorId = f.FloorId,
                    ApartmentId = f.ApartmentId,
                    IsOccupied = f.IsOccupied
                })
                .ToList();
        }

        public async Task<AssignFlatResponseDto> AssignFlatToResidentAsync(AssignFlatDto dto)
        {
            var user = await UserRepo.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception(ErrorMessages.UserNotFound);

            var flat = await FatRepo.GetByIdAsync(dto.FlatId);
            if (flat == null)
                throw new Exception(ErrorMessages.FlatNotFound);

            if (flat.OwnerUserId != null)
                throw new Exception(ErrorMessages.FlatAlreadyOccupied);

            flat.OwnerUserId = user.Id;
            flat.IsOccupied = true;
            user.FlatId = flat.Id;
            user.Status = ResidentStatus.Active;

            await UserRepo.UpdateAsync(user);
            await FatRepo.UpdateAsync(flat);
            await FatRepo.SaveChangesAsync();

            var mapping = new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FlatId = flat.Id,
                RelationshipType = user.ResidentType == ResidentType.Owner ? "Owner" : "Tenant",
                FromDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await UserFlatMappingRepo.AddAsync(mapping);
            await UserFlatMappingRepo.SaveChangesAsync();

            if (!string.IsNullOrEmpty(user.Email))
            {
                await EmailService.SendFlatAssignedToResidentAsync(
                    user.Email,
                    user.FullName,
                    flat.FlatNumber
                );
            }

            return new AssignFlatResponseDto
            {
                UserId = user.Id,
                UserName = user.FullName,
                FlatNumber = flat.FlatNumber,
                Message = SuccessMessages.FlatAssigned
            };
        }
    }
}















