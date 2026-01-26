// Application/Services/ApartmentManagementService.cs
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class ApartmentManagementService : IApartmentManagementService
    {
        private readonly IApartmentRepository _apartmentRepo;
        private readonly IFloorRepository _floorRepo;
        private readonly IFlatRepository _flatRepo;
        private readonly IUserRepository _userRepo;

        public ApartmentManagementService(
            IApartmentRepository apartmentRepo,
            IFloorRepository floorRepo,
            IFlatRepository flatRepo,
            IUserRepository userRepo)
        {
            _apartmentRepo = apartmentRepo;
            _floorRepo = floorRepo;
            _flatRepo = flatRepo;
            _userRepo = userRepo;
        }

        public async Task<CreateApartmentResponseDto> CreateApartmentAsync(CreateApartmentDto dto, Guid createdBy)
        {
            var apartment = new Apartment
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                PinCode = dto.PinCode,
                TotalFloors = dto.TotalFloors,
                FlatsPerFloor = dto.FlatsPerFloor,
                TotalFlats = dto.TotalFloors * dto.FlatsPerFloor,
                Status = ApartmentStatus.UnderConstruction,
                IsActive = true,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _apartmentRepo.AddAsync(apartment);

            var response = new CreateApartmentResponseDto
            {
                ApartmentId = apartment.Id,
                Name = apartment.Name,
                TotalFloors = apartment.TotalFloors,
                TotalFlats = apartment.TotalFlats,
                FloorsCreated = new List<FloorCreatedDto>()
            };

            for (int floorNum = 1; floorNum <= dto.TotalFloors; floorNum++)
            {
                var floor = new Floor
                {
                    Id = Guid.NewGuid(),
                    FloorNumber = floorNum,
                    Name = $"Floor {floorNum}",
                    ApartmentId = apartment.Id
                };

                await _floorRepo.AddAsync(floor);

                var floorCreated = new FloorCreatedDto
                {
                    FloorId = floor.Id,
                    FloorNumber = floorNum,
                    FlatNumbers = new List<string>()
                };

                for (int flatNum = 1; flatNum <= dto.FlatsPerFloor; flatNum++)
                {
                    string flatNumber = $"{floorNum}{flatNum:D2}";

                    var flat = new Flat
                    {
                        Id = Guid.NewGuid(),
                        FlatNumber = flatNumber,
                        Name = $"Flat {flatNumber}",
                        FloorId = floor.Id,
                        ApartmentId = apartment.Id,
                        IsOccupied = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _flatRepo.AddAsync(flat);
                    floorCreated.FlatNumbers.Add(flatNumber);
                }

                response.FloorsCreated.Add(floorCreated);
            }

            return response;
        }

        public async Task<List<ApartmentListDto>> GetAllApartmentsAsync()
        {
            var apartments = await _apartmentRepo.GetAllWithDetailsAsync();
            return apartments.Select(a => new ApartmentListDto
            {
                Id = a.Id,
                Name = a.Name,
                Address = a.Address,
                City = a.City,
                TotalFloors = a.TotalFloors,
                TotalFlats = a.TotalFlats,
                OccupiedFlats = a.Flats.Count(f => f.IsOccupied),
                Status = a.Status.ToString(),
                IsActive = a.IsActive
            }).ToList();
        }

        public async Task<ApartmentDetailDto?> GetApartmentDetailAsync(Guid apartmentId)
        {
            var apartment = await _apartmentRepo.GetByIdWithFullDetailsAsync(apartmentId);
            if (apartment == null) return null;

            var manager = apartment.Managers.FirstOrDefault(m => m.IsActive);
            var president = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == "President" && cm.IsActive);
            var secretary = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == "Secretary" && cm.IsActive);
            var treasurer = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == "Treasurer" && cm.IsActive);

            return new ApartmentDetailDto
            {
                Id = apartment.Id,
                Name = apartment.Name,
                Address = apartment.Address,
                City = apartment.City,
                State = apartment.State,
                PinCode = apartment.PinCode,
                TotalFloors = apartment.TotalFloors,
                FlatsPerFloor = apartment.FlatsPerFloor,
                TotalFlats = apartment.TotalFlats,
                OccupiedFlats = apartment.Flats.Count(f => f.IsOccupied),
                VacantFlats = apartment.Flats.Count(f => !f.IsOccupied),
                Status = apartment.Status.ToString(),
                IsActive = apartment.IsActive,
                Manager = manager != null ? new ManagerInfoDto
                {
                    UserId = manager.UserId,
                    FullName = manager.User.FullName,
                    Email = manager.User.Email,
                    Phone = manager.User.PrimaryPhone,
                    AssignedAt = manager.AssignedAt
                } : null,
                President = president != null ? MapToCommunityLeader(president) : null,
                Secretary = secretary != null ? MapToCommunityLeader(secretary) : null,
                Treasurer = treasurer != null ? MapToCommunityLeader(treasurer) : null,
                CreatedAt = apartment.CreatedAt
            };
        }

        public async Task<ApartmentDiagramDto> GetApartmentDiagramAsync(Guid apartmentId)
        {
            var apartment = await _apartmentRepo.GetByIdWithFloorsAndFlatsAsync(apartmentId);
            if (apartment == null) throw new Exception("Apartment not found");

            var diagram = new ApartmentDiagramDto
            {
                ApartmentId = apartment.Id,
                Name = apartment.Name,
                TotalFloors = apartment.TotalFloors,
                Floors = new List<FloorDiagramDto>()
            };

            foreach (var floor in apartment.Floors.OrderByDescending(f => f.FloorNumber))
            {
                var floorDiagram = new FloorDiagramDto
                {
                    FloorId = floor.Id,
                    FloorNumber = floor.FloorNumber,
                    Name = floor.Name,
                    Flats = new List<FlatDiagramDto>()
                };

                foreach (var flat in floor.Flats.OrderBy(f => f.FlatNumber))
                {
                    var mapping = flat.UserFlatMappings?.FirstOrDefault(m => m.IsActive);

                    floorDiagram.Flats.Add(new FlatDiagramDto
                    {
                        FlatId = flat.Id,
                        FlatNumber = flat.FlatNumber,
                        IsOccupied = flat.IsOccupied,
                        OccupantName = mapping?.User?.FullName,
                        OccupantType = mapping?.RelationshipType,
                        Status = flat.IsOccupied ? "Occupied" : "Vacant"
                    });
                }

                diagram.Floors.Add(floorDiagram);
            }

            return diagram;
        }

        public async Task<bool> AssignManagerAsync(AssignManagerDto dto, Guid assignedBy)
        {
            var user = await _userRepo.GetByIdAsync(dto.UserId);
            if (user == null) throw new Exception("User not found");

            var hasManagerRole = user.UserRoles?.Any(ur => ur.Role.Name == "Manager") ?? false;
            if (!hasManagerRole) throw new Exception("User must have Manager role");

            var existingManager = await _apartmentRepo.GetActiveManagerAsync(dto.ApartmentId);
            if (existingManager != null)
            {
                existingManager.IsActive = false;
                await _apartmentRepo.UpdateManagerAsync(existingManager);
            }

            var newManager = new ApartmentManager
            {
                Id = Guid.NewGuid(),
                ApartmentId = dto.ApartmentId,
                UserId = dto.UserId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _apartmentRepo.AddManagerAsync(newManager);
            return true;
        }

        public async Task<bool> UpdateApartmentAsync(Guid apartmentId, UpdateApartmentDto dto, Guid updatedBy)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(apartmentId);
            if (apartment == null) throw new Exception("Apartment not found");

            apartment.Name = dto.Name;
            apartment.Address = dto.Address;
            apartment.City = dto.City;
            apartment.State = dto.State;
            apartment.PinCode = dto.PinCode;
            apartment.IsActive = dto.IsActive;
            apartment.UpdatedAt = DateTime.UtcNow;
            apartment.UpdatedBy = updatedBy;

            await _apartmentRepo.UpdateAsync(apartment);
            return true;
        }

        public async Task<bool> DeactivateApartmentAsync(Guid apartmentId, Guid deactivatedBy)
        {
            var apartment = await _apartmentRepo.GetByIdAsync(apartmentId);
            if (apartment == null) throw new Exception("Apartment not found");

            apartment.IsActive = false;
            apartment.UpdatedAt = DateTime.UtcNow;
            apartment.UpdatedBy = deactivatedBy;

            await _apartmentRepo.UpdateAsync(apartment);
            return true;
        }

        public async Task<bool> RemoveManagerAsync(Guid apartmentId, Guid userId, Guid removedBy)
        {
            var manager = await _apartmentRepo.GetActiveManagerAsync(apartmentId);
            if (manager == null || manager.UserId != userId)
                throw new Exception("Manager not found");

            manager.IsActive = false;
            await _apartmentRepo.UpdateManagerAsync(manager);
            return true;
        }

        private CommunityLeaderDto MapToCommunityLeader(CommunityMember cm)
        {
            var flatMapping = cm.User.UserFlatMappings?.FirstOrDefault(ufm => ufm.IsActive);
            return new CommunityLeaderDto
            {
                UserId = cm.UserId,
                FullName = cm.User.FullName,
                Email = cm.User.Email,
                FlatNumber = flatMapping?.Flat?.FlatNumber ?? "N/A",
                AssignedAt = cm.AssignedAt
            };
        }
    }
}