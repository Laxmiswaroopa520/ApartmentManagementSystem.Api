// Application/Services/AdminResidentService.cs
using ApartmentManagementSystem.Application.DTOs.Admin;
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
        private readonly IFlatRepository FlatRepo;
        private readonly IUserFlatMappingRepository UserFlatMappingRepo;
        private readonly IEmailService EmailService;

        public AdminResidentService(
            IUserRepository userRepository,
            IFlatRepository flatRepository,
            IUserFlatMappingRepository userFlatMappingRepository,
            IEmailService emailService)
        {
            UserRepo = userRepository;
            FlatRepo = flatRepository;
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

        public async Task<AssignFlatResponseDto> AssignFlatToResidentAsync(AssignFlatDto dto)
        {
            var user = await UserRepo.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception(ErrorMessages.UserNotFound);

            var flat = await FlatRepo.GetByIdAsync(dto.FlatId);
            if (flat == null)
                throw new Exception(ErrorMessages.FlatNotFound);

            if (flat.OwnerUserId != null)
                throw new Exception(ErrorMessages.FlatAlreadyOccupied);

            flat.OwnerUserId = user.Id;
            flat.IsOccupied = true;

            user.FlatId = flat.Id;
            user.Status = ResidentStatus.Active;

            await UserRepo.UpdateAsync(user);
            await FlatRepo.UpdateAsync(flat);
            await FlatRepo.SaveChangesAsync();

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

        public async Task<List<FloorDto>> GetAllFloorsAsync()
        {
            var floors = await FlatRepo.GetAllFloorsAsync();
            return floors.Select(f => new FloorDto
            {
                Id = f.Id,
                FloorNumber = f.FloorNumber
            }).ToList();
        }

        public async Task<List<FlatDto>> GetVacantFlatsByFloorAsync(Guid floorId)
        {
            var flats = await FlatRepo.GetVacantFlatsByFloorAsync(floorId);
            return flats.Select(f => new FlatDto
            {
                Id = f.Id,
                FlatNumber = f.FlatNumber
            }).ToList();
        }
    }
}