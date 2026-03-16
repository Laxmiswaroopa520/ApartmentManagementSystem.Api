using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for all apartment lifecycle operations.
    ///
    /// Handles:
    /// - Creating apartments with auto-generated floors and flats (bulk insert)
    /// - Retrieving apartment lists, details, and visual diagrams
    /// - Updating and deactivating apartments
    /// - Hard-deleting apartments (cascade) when no flats are occupied
    /// - Assigning and removing apartment managers
    /// </summary>
    public class ApartmentManagementService : IApartmentManagementService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>
        /// Initialises ApartmentManagementService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        public ApartmentManagementService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        /// <summary>
        /// Creates a new apartment and auto-generates all floors and flats.
        ///
        /// All entities (apartment, floors, flats) are built in memory first
        /// and committed in a single SaveChanges call — avoiding N+1 DB inserts.
        /// </summary>
        /// <param name="dto">Apartment creation data including floor and flat counts.</param>
        /// <param name="createdBy">UserId of the admin performing the creation.</param>
        /// <returns>Response DTO containing apartment ID, totals, and all created floor/flat numbers.</returns>
        public async Task<CreateApartmentResponseDto> CreateApartmentAsync(
            CreateApartmentDto dto, Guid createdBy)
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

            await UoW.Apartments.AddAsync(apartment);

            var floors = Enumerable.Range(1, dto.TotalFloors)
                .Select(floorNum => new Floor
                {
                    Id = Guid.NewGuid(),
                    FloorNumber = floorNum,
                    Name = $"Floor {floorNum}",
                    ApartmentId = apartment.Id
                }).ToList();

            var flats = floors.SelectMany(floor =>
                Enumerable.Range(1, dto.FlatsPerFloor).Select(flatNum => new Flat
                {
                    Id = Guid.NewGuid(),
                    FlatNumber = $"{floor.FloorNumber}{flatNum:D2}",
                    Name = $"Flat {floor.FloorNumber}{flatNum:D2}",
                    FloorId = floor.Id,
                    ApartmentId = apartment.Id,
                    IsOccupied = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                })
            ).ToList();

            await UoW.Floors.AddRangeAsync(floors);
            await UoW.Flats.AddRangeAsync(flats);

            // ONE SaveChanges for apartment + all floors + all flats
            await UoW.SaveChangesAsync();

            return new CreateApartmentResponseDto
            {
                ApartmentId = apartment.Id,
                Name = apartment.Name,
                TotalFloors = apartment.TotalFloors,
                TotalFlats = apartment.TotalFlats,
                FloorsCreated = floors.Select(floor => new FloorCreatedDto
                {
                    FloorId = floor.Id,
                    FloorNumber = floor.FloorNumber,
                    FlatNumbers = flats
                        .Where(f => f.FloorId == floor.Id)
                        .Select(f => f.FlatNumber)
                        .ToList()
                }).ToList()
            };
        }

        /// <summary>
        /// Retrieves a summary list of all active apartments including flat counts.
        /// </summary>
        /// <returns>List of apartment list DTOs.</returns>
        public async Task<List<ApartmentListDto>> GetAllApartmentsAsync()
        {
            var apartments = await UoW.Apartments.GetAllWithDetailsAsync();
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

        /// <summary>
        /// Retrieves full details for a single apartment including manager
        /// and all community leader assignments (President, Secretary, Treasurer).
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        /// <returns>Detailed apartment DTO or null if not found.</returns>
        public async Task<ApartmentDetailDto?> GetApartmentDetailAsync(Guid apartmentId)
        {
            var apartment = await UoW.Apartments.GetByIdWithFullDetailsAsync(apartmentId);
            if (apartment == null) return null;

            var manager = apartment.Managers.FirstOrDefault(m => m.IsActive);
            var president = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == SystemRoles.President && cm.IsActive);
            var secretary = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == SystemRoles.Secretary && cm.IsActive);
            var treasurer = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == SystemRoles.Treasurer && cm.IsActive);

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

        /// <summary>
        /// Builds a visual floor-by-floor diagram of an apartment
        /// showing each flat's occupancy status and current occupant details.
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        /// <returns>Apartment diagram DTO with floors and flats ordered numerically.</returns>
        /// <exception cref="Exception">
        /// Thrown when the apartment, its floors, or its flats are not found.
        /// </exception>
        public async Task<ApartmentDiagramDto> GetApartmentDiagramAsync(Guid apartmentId)
        {
            var apartment = await UoW.Apartments.GetByIdWithFloorsAndFlatsAsync(apartmentId)
                ?? throw new Exception(ErrorMessages.ApartmentNotFound);

            if (apartment.Floors == null || !apartment.Floors.Any())
                throw new Exception(ErrorMessages.NoFloorsFound);

            var diagram = new ApartmentDiagramDto
            {
                ApartmentId = apartment.Id,
                Name = apartment.Name,
                Address = apartment.Address,
                TotalFloors = apartment.TotalFloors,
                Floors = new List<FloorDiagramDto>()
            };

            foreach (var floor in apartment.Floors.OrderBy(f => f.FloorNumber))
            {
                var floorDiagram = new FloorDiagramDto
                {
                    FloorId = floor.Id,
                    FloorNumber = floor.FloorNumber,
                    Name = floor.Name ?? $"Floor {floor.FloorNumber}",
                    Flats = new List<FlatDiagramDto>()
                };

                if (floor.Flats != null && floor.Flats.Any())
                {
                    foreach (var flat in floor.Flats.OrderBy(f => f.FlatNumber))
                    {
                        var mapping = flat.UserFlatMappings?.FirstOrDefault(m => m.IsActive);
                        floorDiagram.Flats.Add(new FlatDiagramDto
                        {
                            FlatId = flat.Id,
                            FlatNumber = flat.FlatNumber ?? "N/A",
                            IsOccupied = flat.IsOccupied,
                            OccupantName = mapping?.User?.FullName,
                            OccupantType = mapping?.RelationshipType,
                            Status = flat.IsOccupied ? "Occupied" : "Vacant"
                        });
                    }
                }

                diagram.Floors.Add(floorDiagram);
            }

            if (!diagram.Floors.Any())
                throw new Exception(ErrorMessages.DiagramNoFloors);

            if (!diagram.Floors.First().Flats.Any())
                throw new Exception(ErrorMessages.DiagramNoFlats);

            return diagram;
        }

        /// <summary>
        /// Assigns a user as the active manager of an apartment.
        ///
        /// If another manager is currently active for the same apartment,
        /// they are deactivated first. All changes are committed in one SaveChanges.
        /// </summary>
        /// <param name="dto">DTO containing UserId and ApartmentId.</param>
        /// <param name="assignedBy">UserId of the admin performing the assignment.</param>
        /// <returns>True on success.</returns>
        /// <exception cref="Exception">Thrown when user not found or user lacks Manager role.</exception>
        public async Task<bool> AssignManagerAsync(AssignManagerDto dto, Guid assignedBy)
        {
            var user = await UoW.Users.GetByIdAsync(dto.UserId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var hasManagerRole = user.UserRoles?.Any(ur => ur.Role.Name == SystemRoles.Manager) ?? false;
            if (!hasManagerRole)
                throw new Exception(ErrorMessages.UserMustHaveManagerRole);

            var existingManager = await UoW.Apartments.GetActiveManagerAsync(dto.ApartmentId);
            if (existingManager != null)
            {
                existingManager.IsActive = false;
                UoW.Apartments.UpdateManager(existingManager);
            }

            await UoW.Apartments.AddManagerAsync(new ApartmentManager
            {
                Id = Guid.NewGuid(),
                ApartmentId = dto.ApartmentId,
                UserId = dto.UserId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            });

            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Removes the active manager from an apartment by setting their record inactive.
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        /// <param name="userId">UserId of the manager to remove.</param>
        /// <param name="removedBy">UserId of the admin performing the removal.</param>
        /// <returns>True on success.</returns>
        /// <exception cref="Exception">Thrown when no matching active manager is found.</exception>
        public async Task<bool> RemoveManagerAsync(Guid apartmentId, Guid userId, Guid removedBy)
        {
            var manager = await UoW.Apartments.GetActiveManagerAsync(apartmentId);
            if (manager == null || manager.UserId != userId)
                throw new Exception(ErrorMessages.ManagerNotFound);

            manager.IsActive = false;
            UoW.Apartments.UpdateManager(manager);
            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates editable fields of an existing apartment record.
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment to update.</param>
        /// <param name="dto">Updated apartment values.</param>
        /// <param name="updatedBy">UserId of the admin performing the update.</param>
        /// <returns>True on success.</returns>
        /// <exception cref="Exception">Thrown when the apartment is not found.</exception>
        public async Task<bool> UpdateApartmentAsync(
            Guid apartmentId, UpdateApartmentDto dto, Guid updatedBy)
        {
            var apartment = await UoW.Apartments.GetByIdAsync(apartmentId)
                ?? throw new Exception(ErrorMessages.ApartmentNotFound);

            apartment.Name = dto.Name;
            apartment.Address = dto.Address;
            apartment.City = dto.City;
            apartment.State = dto.State;
            apartment.PinCode = dto.PinCode;
            apartment.IsActive = dto.IsActive;
            apartment.UpdatedAt = DateTime.UtcNow;
            apartment.UpdatedBy = updatedBy;

            UoW.Apartments.Update(apartment);
            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Soft-deactivates an apartment by setting IsActive to false.
        /// The apartment record and all its children are retained in the database.
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment to deactivate.</param>
        /// <param name="deactivatedBy">UserId of the admin performing the deactivation.</param>
        /// <returns>True on success.</returns>
        /// <exception cref="Exception">Thrown when the apartment is not found.</exception>
        public async Task<bool> DeactivateApartmentAsync(Guid apartmentId, Guid deactivatedBy)
        {
            var apartment = await UoW.Apartments.GetByIdAsync(apartmentId)
                ?? throw new Exception(ErrorMessages.ApartmentNotFound);

            apartment.IsActive = false;
            apartment.UpdatedAt = DateTime.UtcNow;
            apartment.UpdatedBy = deactivatedBy;

            UoW.Apartments.Update(apartment);
            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Hard-deletes an apartment and all its child records
        /// (floors, flats, mappings, managers, community members).
        ///
        /// Deletion is blocked if any flat within the apartment is currently occupied.
        /// All cascade deletes are staged in memory and committed in one SaveChanges.
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment to delete.</param>
        /// <param name="deletedBy">UserId of the admin performing the deletion.</param>
        /// <returns>True on success.</returns>
        /// <exception cref="Exception">
        /// Thrown when the apartment is not found or has occupied flats.
        /// </exception>
        public async Task<bool> DeleteApartmentAsync(Guid apartmentId, Guid deletedBy)
        {
            var apartment = await UoW.Apartments.GetByIdAsync(apartmentId)
                ?? throw new Exception(ErrorMessages.ApartmentNotFound);

            var full = await UoW.Apartments.GetByIdWithFloorsAndFlatsAsync(apartmentId);
            if (full?.Floors != null)
            {
                var hasOccupied = full.Floors
                    .SelectMany(f => f.Flats)
                    .Any(fl => fl.IsOccupied);

                if (hasOccupied)
                    throw new Exception(ErrorMessages.ApartmentHasOccupants);
            }

            await UoW.Apartments.PrepareDeleteAsync(apartment);
            await UoW.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// Maps a CommunityMember entity to a CommunityLeaderDto,
        /// resolving the leader's active flat number from their flat mappings.
        /// </summary>
        /// <param name="cm">The community member entity to map.</param>
        /// <returns>Populated CommunityLeaderDto.</returns>
        private static CommunityLeaderDto MapToCommunityLeader(CommunityMember cm)
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


















































/*using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class ApartmentManagementService : IApartmentManagementService
    {
        private readonly IApartmentRepository ApartmentRepo;
        private readonly IFloorRepository FloorRepo;
        private readonly IFlatRepository FlatRepo;
        private readonly IUserRepository UserRepo;

        public ApartmentManagementService(
            IApartmentRepository apartmentRepo,
            IFloorRepository floorRepo,
            IFlatRepository flatRepo,
            IUserRepository userRepo)
        {
            ApartmentRepo = apartmentRepo;
            FloorRepo = floorRepo;
            FlatRepo = flatRepo;
            UserRepo = userRepo;
        }
        public async Task<CreateApartmentResponseDto> CreateApartmentAsync(
    CreateApartmentDto dto, Guid createdBy)
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
            await ApartmentRepo.AddAsync(apartment);

            var response = new CreateApartmentResponseDto
            {
                ApartmentId = apartment.Id,
                Name = apartment.Name,
                TotalFloors = apartment.TotalFloors,
                TotalFlats = apartment.TotalFlats,
                FloorsCreated = new List<FloorCreatedDto>()
            };

            foreach (int floorNum in Enumerable.Range(1, dto.TotalFloors))
            {
                var floor = new Floor
                {
                    Id = Guid.NewGuid(),
                    FloorNumber = floorNum,
                    Name = $"Floor {floorNum}",
                    ApartmentId = apartment.Id
                };
                await FloorRepo.AddAsync(floor);

                var floorCreated = new FloorCreatedDto
                {
                    FloorId = floor.Id,
                    FloorNumber = floorNum,
                    FlatNumbers = new List<string>()
                };

                foreach (int flatNum in Enumerable.Range(1, dto.FlatsPerFloor))
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
                    await FlatRepo.AddAsync(flat);
                    floorCreated.FlatNumbers.Add(flatNumber);
                }

                response.FloorsCreated.Add(floorCreated);
            }

            return response;
        }
        /*
        // CREATE 
        public async Task<CreateApartmentResponseDto> CreateApartmentAsync(
            CreateApartmentDto dto, Guid createdBy)
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

            await ApartmentRepo.AddAsync(apartment);

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

                await FloorRepo.AddAsync(floor);

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

                    await FlatRepo.AddAsync(flat);
                    floorCreated.FlatNumbers.Add(flatNumber);
                }

                response.FloorsCreated.Add(floorCreated);
            }

            return response;
        }---------------

//GET ALL
public async Task<List<ApartmentListDto>> GetAllApartmentsAsync()
        {
            var apartments = await ApartmentRepo.GetAllWithDetailsAsync();

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

        // GET DETAIL 
        public async Task<ApartmentDetailDto?> GetApartmentDetailAsync(Guid apartmentId)
        {
            var apartment = await ApartmentRepo.GetByIdWithFullDetailsAsync(apartmentId);
            if (apartment == null) return null;

            //var president = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == "President" && cm.IsActive);
            //var secretary = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == "Secretary" && cm.IsActive);
            // var treasurer = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == "Treasurer" && cm.IsActive);
            var manager = apartment.Managers.FirstOrDefault(m => m.IsActive);
            var president = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == SystemRoles.President && cm.IsActive);
            var secretary = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == SystemRoles.Secretary && cm.IsActive);
            var treasurer = apartment.CommunityMembers.FirstOrDefault(cm => cm.CommunityRole == SystemRoles.Treasurer && cm.IsActive);

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

        // GET DIAGRAM
        public async Task<ApartmentDiagramDto> GetApartmentDiagramAsync(Guid apartmentId)
        {
            Console.WriteLine($"=== GetApartmentDiagramAsync Called === ApartmentID: {apartmentId}");

            var apartment = await ApartmentRepo.GetByIdWithFloorsAndFlatsAsync(apartmentId);

            if (apartment == null)
            {
                Console.WriteLine("ERROR: Apartment not found!");
                throw new Exception(ErrorMessages.ApartmentNotFound);          
            }

            Console.WriteLine($"Apartment found: {apartment.Name}, Floors: {apartment.Floors?.Count ?? 0}");

            if (apartment.Floors == null || !apartment.Floors.Any())
            {
                Console.WriteLine("ERROR: No floors found!");
                throw new Exception(ErrorMessages.NoFloorsFound);              
            }

            var diagram = new ApartmentDiagramDto
            {
                ApartmentId = apartment.Id,
                Name = apartment.Name,
                Address = apartment.Address,
                TotalFloors = apartment.TotalFloors,
                Floors = new List<FloorDiagramDto>()
            };

            foreach (var floor in apartment.Floors.OrderBy(f => f.FloorNumber))
            {
                Console.WriteLine($"Processing Floor {floor.FloorNumber}, Flats: {floor.Flats?.Count ?? 0}");

                var floorDiagram = new FloorDiagramDto
                {
                    FloorId = floor.Id,
                    FloorNumber = floor.FloorNumber,
                    Name = floor.Name ?? $"Floor {floor.FloorNumber}",
                    Flats = new List<FlatDiagramDto>()
                };

                if (floor.Flats != null && floor.Flats.Any())
                {
                    foreach (var flat in floor.Flats.OrderBy(f => f.FlatNumber))
                    {
                        var mapping = flat.UserFlatMappings?.FirstOrDefault(m => m.IsActive);

                        floorDiagram.Flats.Add(new FlatDiagramDto
                        {
                            FlatId = flat.Id,
                            FlatNumber = flat.FlatNumber ?? "N/A",
                            IsOccupied = flat.IsOccupied,
                            OccupantName = mapping?.User?.FullName,
                            OccupantType = mapping?.RelationshipType,
                            Status = flat.IsOccupied ? "Occupied" : "Vacant"
                        });
                    }
                }
                else
                {
                    Console.WriteLine($"WARNING: Floor {floor.FloorNumber} has no flats!");
                }

                diagram.Floors.Add(floorDiagram);
            }

            Console.WriteLine($"=== Diagram Created === Floors: {diagram.Floors.Count}, First Floor Flats: {diagram.Floors.FirstOrDefault()?.Flats.Count ?? 0}");

            if (!diagram.Floors.Any())
                throw new Exception(ErrorMessages.DiagramNoFloors);            

            if (!diagram.Floors.First().Flats.Any())
                throw new Exception(ErrorMessages.DiagramNoFlats);             

            return diagram;
        }

        // ASSIGN MANAGER 
        public async Task<bool> AssignManagerAsync(AssignManagerDto dto, Guid assignedBy)
        {
            var user = await UserRepo.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception(ErrorMessages.UserNotFound);              

            var hasManagerRole = user.UserRoles?.Any(ur => ur.Role.Name == SystemRoles.Manager) ?? false;
            if (!hasManagerRole)
                throw new Exception(ErrorMessages.UserMustHaveManagerRole);    

            var existingManager = await ApartmentRepo.GetActiveManagerAsync(dto.ApartmentId);
            if (existingManager != null)
            {
                existingManager.IsActive = false;
                await ApartmentRepo.UpdateManagerAsync(existingManager);
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

            await ApartmentRepo.AddManagerAsync(newManager);
            return true;
        }

        // REMOVE MANAGER
        public async Task<bool> RemoveManagerAsync(Guid apartmentId, Guid userId, Guid removedBy)
        {
            var manager = await ApartmentRepo.GetActiveManagerAsync(apartmentId);
            if (manager == null || manager.UserId != userId)
                throw new Exception(ErrorMessages.ManagerNotFound);            

            manager.IsActive = false;
            await ApartmentRepo.UpdateManagerAsync(manager);
            return true;
        }

        // UPDATE
        public async Task<bool> UpdateApartmentAsync(
            Guid apartmentId, UpdateApartmentDto dto, Guid updatedBy)
        {
            var apartment = await ApartmentRepo.GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new Exception(ErrorMessages.ApartmentNotFound);          

            apartment.Name = dto.Name;
            apartment.Address = dto.Address;
            apartment.City = dto.City;
            apartment.State = dto.State;
            apartment.PinCode = dto.PinCode;
            apartment.IsActive = dto.IsActive;
            apartment.UpdatedAt = DateTime.UtcNow;
            apartment.UpdatedBy = updatedBy;

            await ApartmentRepo.UpdateAsync(apartment);
            return true;
        }

        // Deactivate Apartment
        public async Task<bool> DeactivateApartmentAsync(Guid apartmentId, Guid deactivatedBy)
        {
            var apartment = await ApartmentRepo.GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new Exception(ErrorMessages.ApartmentNotFound);          

            apartment.IsActive = false;
            apartment.UpdatedAt = DateTime.UtcNow;
            apartment.UpdatedBy = deactivatedBy;

            await ApartmentRepo.UpdateAsync(apartment);
            return true;
        }

        // DELETE 
        public async Task<bool> DeleteApartmentAsync(Guid apartmentId, Guid deletedBy)
        {
            var apartment = await ApartmentRepo.GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new Exception(ErrorMessages.ApartmentNotFound);          

            // Block delete if any flat is occupied
            var full = await ApartmentRepo.GetByIdWithFloorsAndFlatsAsync(apartmentId);
            if (full?.Floors != null)
            {
                var hasOccupied = full.Floors
                    .SelectMany(f => f.Flats)
                    .Any(fl => fl.IsOccupied);

                if (hasOccupied)
                    throw new Exception(ErrorMessages.ApartmentHasOccupants);  
            }

            await ApartmentRepo.DeleteAsync(apartment);
            return true;
        }

        // PRIVATE HELPERS 
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
*/
















