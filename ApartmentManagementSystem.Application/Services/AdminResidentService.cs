using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for admin-side resident management operations.
    ///
    /// Handles:
    /// - Retrieving residents pending flat assignment
    /// - Providing apartment/floor/flat dropdown data scoped to the caller's role
    /// - Assigning flats to residents and updating related records atomically
    /// </summary>
    public class AdminResidentService : IAdminResidentService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>Email service for sending flat assignment notifications.</summary>
        private readonly IEmailService EmailService;

        /// <summary>
        /// Initialises AdminResidentService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        /// <param name="emailService">Service for sending email notifications.</param>
        public AdminResidentService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            UoW = unitOfWork;
            EmailService = emailService;
        }

        /// <summary>
        /// Retrieves all residents whose status is PendingFlatAllocation.
        /// These residents have completed OTP verification and registration
        /// but have not yet been assigned a flat.
        /// </summary>
        /// <returns>List of pending resident DTOs.</returns>
        public async Task<List<PendingResidentDto>> GetPendingResidentsAsync()
        {
            var users = await UoW.Users.GetPendingResidentsAsync();
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

        /// <summary>
        /// Returns the list of apartments visible to a user based on their role.
        ///
        /// - SuperAdmin: sees all active apartments.
        /// - Manager: sees only the apartment they are currently assigned to manage.
        /// - Other roles: returns an empty list.
        /// </summary>
        /// <param name="userId">Unique identifier of the requesting user.</param>
        /// <param name="role">Role name of the requesting user.</param>
        /// <returns>List of apartment dropdown DTOs.</returns>
        public async Task<List<ApartmentDropdownDto>> GetApartmentsForUserAsync(Guid userId, string role)
        {
            if (role == "SuperAdmin")
            {
                var all = await UoW.Apartments.GetAllAsync();
                return all
                    .Where(a => a.IsActive)
                    .Select(a => new ApartmentDropdownDto { Id = a.Id, Name = a.Name })
                    .OrderBy(a => a.Name)
                    .ToList();
            }
            else if (role == "Manager")
            {
                var manager = await UoW.Apartments.GetActiveManagerByUserIdAsync(userId);
                if (manager != null)
                {
                    var apartment = await UoW.Apartments.GetByIdAsync(manager.ApartmentId);
                    if (apartment != null && apartment.IsActive)
                        return new List<ApartmentDropdownDto>
                        {
                            new() { Id = apartment.Id, Name = apartment.Name }
                        };
                }
            }

            return new List<ApartmentDropdownDto>();
        }

        /// <summary>
        /// Retrieves all floors belonging to a specific apartment,
        /// ordered by floor number ascending.
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        /// <returns>List of floor DTOs.</returns>
        public async Task<List<FloorDto>> GetFloorsByApartmentAsync(Guid apartmentId)
        {
            var floors = await UoW.Floors.GetByApartmentIdAsync(apartmentId);
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

        /// <summary>
        /// Retrieves all vacant (unoccupied, active) flats on a specific floor,
        /// ordered by flat number ascending.
        /// </summary>
        /// <param name="floorId">Unique identifier of the floor.</param>
        /// <returns>List of vacant flat DTOs.</returns>
        public async Task<List<FlatDto>> GetVacantFlatsByFloorAsync(Guid floorId)
        {
            var flats = await UoW.Flats.GetVacantFlatsByFloorAsync(floorId);
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

        /// <summary>
        /// Assigns a flat to a pending resident.
        ///
        /// Operations performed atomically in one SaveChanges:
        /// - Sets flat as occupied and links it to the resident.
        /// - Updates resident status to Active and records their FlatId.
        /// - Creates a UserFlatMapping record with the correct relationship type.
        ///
        /// Sends a flat assignment email notification if the resident has an email address.
        /// </summary>
        /// <param name="dto">DTO containing UserId and FlatId to assign.</param>
        /// <returns>Assignment response with user name, flat number, and success message.</returns>
        /// <exception cref="Exception">
        /// Thrown when the user or flat is not found, or the flat is already occupied.
        /// </exception>
        public async Task<AssignFlatResponseDto> AssignFlatToResidentAsync(AssignFlatDto dto)
        {
            var user = await UoW.Users.GetByIdAsync(dto.UserId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var flat = await UoW.Flats.GetByIdAsync(dto.FlatId)
                ?? throw new Exception(ErrorMessages.FlatNotFound);

            if (flat.OwnerUserId != null)
                throw new Exception(ErrorMessages.FlatAlreadyOccupied);

            flat.OwnerUserId = user.Id;
            flat.IsOccupied = true;
            user.FlatId = flat.Id;
            user.Status = ResidentStatus.Active;

            UoW.Users.Update(user);
            UoW.Flats.Update(flat);

            await UoW.UserFlatMappings.AddAsync(new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FlatId = flat.Id,
                RelationshipType = user.ResidentType == ResidentType.Owner ? "Owner" : "Tenant",
                FromDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            // ONE SaveChanges commits user + flat + mapping together
            await UoW.SaveChangesAsync();

            if (!string.IsNullOrEmpty(user.Email))
                await EmailService.SendFlatAssignedToResidentAsync(user.Email, user.FullName, flat.FlatNumber);

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











































/*using ApartmentManagementSystem.Application.DTOs.Admin;
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.DTOs.Onboarding;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class AdminResidentService : IAdminResidentService
    {
        private readonly IUnitOfWork UoW;
        private readonly IEmailService EmailService;

        public AdminResidentService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            UoW = unitOfWork;
            EmailService = emailService;
        }

        public async Task<List<PendingResidentDto>> GetPendingResidentsAsync()
        {
            var users = await UoW.Users.GetPendingResidentsAsync();
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

        public async Task<List<ApartmentDropdownDto>> GetApartmentsForUserAsync(Guid userId, string role)
        {
            if (role == "SuperAdmin")
            {
                var all = await UoW.Apartments.GetAllAsync();
                return all
                    .Where(a => a.IsActive)
                    .Select(a => new ApartmentDropdownDto { Id = a.Id, Name = a.Name })
                    .OrderBy(a => a.Name)
                    .ToList();
            }
            else if (role == "Manager")
            {
                var manager = await UoW.Apartments.GetActiveManagerByUserIdAsync(userId);
                if (manager != null)
                {
                    var apartment = await UoW.Apartments.GetByIdAsync(manager.ApartmentId);
                    if (apartment != null && apartment.IsActive)
                        return new List<ApartmentDropdownDto>
                        {
                            new() { Id = apartment.Id, Name = apartment.Name }
                        };
                }
            }

            return new List<ApartmentDropdownDto>();
        }

        public async Task<List<FloorDto>> GetFloorsByApartmentAsync(Guid apartmentId)
        {
            var floors = await UoW.Floors.GetByApartmentIdAsync(apartmentId);
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
            var flats = await UoW.Flats.GetVacantFlatsByFloorAsync(floorId);
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
            var user = await UoW.Users.GetByIdAsync(dto.UserId)
                ?? throw new Exception(ErrorMessages.UserNotFound);

            var flat = await UoW.Flats.GetByIdAsync(dto.FlatId)
                ?? throw new Exception(ErrorMessages.FlatNotFound);

            if (flat.OwnerUserId != null)
                throw new Exception(ErrorMessages.FlatAlreadyOccupied);

            // Mutate in memory
            flat.OwnerUserId = user.Id;
            flat.IsOccupied = true;
            user.FlatId = flat.Id;
            user.Status = ResidentStatus.Active;

            UoW.Users.Update(user);
            UoW.Flats.Update(flat);

            await UoW.UserFlatMappings.AddAsync(new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FlatId = flat.Id,
                RelationshipType = user.ResidentType == ResidentType.Owner ? "Owner" : "Tenant",
                FromDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            // ONE SaveChanges for user + flat + mapping
            await UoW.SaveChangesAsync();

            if (!string.IsNullOrEmpty(user.Email))
                await EmailService.SendFlatAssignedToResidentAsync(user.Email, user.FullName, flat.FlatNumber);

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

*/


















































