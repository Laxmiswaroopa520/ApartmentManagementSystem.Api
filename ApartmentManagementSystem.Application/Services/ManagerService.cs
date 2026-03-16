using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for apartment manager assignment operations.
    ///
    /// Handles:
    /// - Listing resident owners within an apartment who are eligible for manager assignment
    /// - Assigning managers from existing residents or as new external users
    /// - Removing a manager from an apartment
    ///
    /// Supports two assignment modes:
    /// - Internal: assigns an existing resident owner from the apartment.
    /// - External: creates a new minimal user record for an external manager.
    /// </summary>
    public class ManagerService : IManagerService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>
        /// Initialises ManagerService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        public ManagerService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        /// <summary>
        /// Retrieves resident owners in a specific apartment who can be assigned as manager.
        ///
        /// Filters out residents who are already the active manager of this apartment.
        /// Includes residents who manage a different apartment (to allow re-assignment).
        /// </summary>
        /// <param name="apartmentId">Unique identifier of the apartment.</param>
        /// <returns>List of available manager candidate DTOs, ordered by full name.</returns>
        public async Task<List<AvailableManagerDto>> GetApartmentResidentsForManagerAssignmentAsync(
            Guid apartmentId)
        {
            var residentOwners = await UoW.Users.GetUsersByRoleWithFlatsAsync("ResidentOwner");
            var result = new List<AvailableManagerDto>();

            foreach (var resident in residentOwners)
            {
                var flatInApartment = resident.UserFlatMappings
                    ?.FirstOrDefault(ufm => ufm.IsActive && ufm.Flat?.ApartmentId == apartmentId);

                if (flatInApartment == null) continue;

                var currentAssignment = await UoW.Apartments.GetActiveManagerByUserIdAsync(resident.Id);
                if (currentAssignment != null && currentAssignment.ApartmentId == apartmentId)
                    continue;

                result.Add(new AvailableManagerDto
                {
                    UserId = resident.Id,
                    FullName = resident.FullName,
                    Email = resident.Email ?? "",
                    Phone = resident.PrimaryPhone,
                    FlatNumber = flatInApartment.Flat?.FlatNumber ?? "N/A",
                    IsCurrentlyAssigned = currentAssignment != null,
                    CurrentApartmentName = currentAssignment?.Apartment?.Name
                });
            }

            return result.OrderBy(r => r.FullName).ToList();
        }

        /// <summary>
        /// Assigns a manager to an apartment.
        ///
        /// Supports two modes controlled by dto.IsExternalManager:
        /// - False (internal): uses an existing resident from the apartment.
        /// - True (external): creates a new minimal user record with no roles.
        ///
        /// Before assigning:
        /// - Deactivates any existing manager of the target apartment.
        /// - Deactivates this user from any other apartment they currently manage.
        ///
        /// All changes are staged and committed in one SaveChanges.
        /// </summary>
        /// <param name="dto">Assignment request containing manager details and apartment ID.</param>
        /// <param name="assignedBy">UserId of the admin performing the assignment.</param>
        /// <returns>Manager assignment DTO with apartment and user details.</returns>
        /// <exception cref="Exception">
        /// Thrown on missing required external manager fields, user not found, or apartment not found.
        /// </exception>
        public async Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(
            AssignManagerRequestDto dto, Guid assignedBy)
        {
            Guid targetUserId;
            User targetUser;

            if (dto.IsExternalManager)
            {
                if (string.IsNullOrWhiteSpace(dto.ExternalManagerName))
                    throw new Exception(ManagerMessages.ManagerNameRequired);

                if (string.IsNullOrWhiteSpace(dto.ExternalManagerPhone))
                    throw new Exception(ManagerMessages.ManagerPhoneRequired);

                var existingUser = await UoW.Users.GetByPhoneAsync(dto.ExternalManagerPhone.Trim());

                if (existingUser != null)
                {
                    targetUser = existingUser;
                    targetUserId = existingUser.Id;
                }
                else
                {
                    targetUser = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = dto.ExternalManagerName.Trim(),
                        PrimaryPhone = dto.ExternalManagerPhone.Trim(),
                        Email = string.IsNullOrWhiteSpace(dto.ExternalManagerEmail)
                                                    ? null
                                                    : dto.ExternalManagerEmail.Trim(),
                        IsActive = true,
                        IsOtpVerified = true,
                        IsRegistrationCompleted = true,
                        Status = ResidentStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };

                    await UoW.Users.AddAsync(targetUser);
                    targetUserId = targetUser.Id;
                }
            }
            else
            {
                if (!dto.UserId.HasValue || dto.UserId == Guid.Empty)
                    throw new Exception(ResidentMessages.SelectResident);

                targetUser = await UoW.Users.GetByIdAsync(dto.UserId.Value)
                    ?? throw new Exception(ResidentMessages.ResidentNotFound);
                targetUserId = dto.UserId.Value;
            }

            var apartment = await UoW.Apartments.GetByIdAsync(dto.ApartmentId)
                ?? throw new Exception(ErrorMessages.ApartmentNotFound);

            var existingManager = await UoW.Apartments.GetActiveManagerAsync(dto.ApartmentId);
            if (existingManager != null)
            {
                existingManager.IsActive = false;
                UoW.Apartments.UpdateManager(existingManager);
            }

            var userOtherAssignment = await UoW.Apartments.GetActiveManagerByUserIdAsync(targetUserId);
            if (userOtherAssignment != null)
            {
                userOtherAssignment.IsActive = false;
                UoW.Apartments.UpdateManager(userOtherAssignment);
            }

            var newManager = new ApartmentManager
            {
                Id = Guid.NewGuid(),
                ApartmentId = dto.ApartmentId,
                UserId = targetUserId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await UoW.Apartments.AddManagerAsync(newManager);
            await UoW.SaveChangesAsync();

            return new ManagerAssignmentDto
            {
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                UserId = targetUser.Id,
                FullName = targetUser.FullName,
                Email = targetUser.Email ?? "",
                Phone = targetUser.PrimaryPhone,
                AssignedAt = newManager.AssignedAt
            };
        }

        /// <summary>
        /// Removes the active manager from an apartment by soft-deactivating their record.
        /// </summary>
        /// <param name="dto">Request DTO containing the apartment ID.</param>
        /// <param name="removedBy">UserId of the admin performing the removal.</param>
        /// <returns>True on success.</returns>
        /// <exception cref="Exception">Thrown when no active manager exists for the apartment.</exception>
        public async Task<bool> RemoveManagerFromApartmentAsync(
            RemoveManagerRequestDto dto, Guid removedBy)
        {
            var manager = await UoW.Apartments.GetActiveManagerAsync(dto.ApartmentId)
                ?? throw new Exception(ManagerMessages.NoActiveManager);

            manager.IsActive = false;
            UoW.Apartments.UpdateManager(manager);
            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>Placeholder — returns empty list until full implementation.</summary>
        public async Task<List<ManagerListDto>> GetAllManagersAsync() =>
            new List<ManagerListDto>();

        /// <summary>Placeholder — returns null until full implementation.</summary>
        public async Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId) =>
            null;
    }
}































/*using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IUnitOfWork UoW;

        public ManagerService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public async Task<List<AvailableManagerDto>> GetApartmentResidentsForManagerAssignmentAsync(
            Guid apartmentId)
        {
            var residentOwners = await UoW.Users.GetUsersByRoleWithFlatsAsync("ResidentOwner");
            var result = new List<AvailableManagerDto>();

            foreach (var resident in residentOwners)
            {
                var flatInApartment = resident.UserFlatMappings
                    ?.FirstOrDefault(ufm => ufm.IsActive && ufm.Flat?.ApartmentId == apartmentId);

                if (flatInApartment == null) continue;

                var currentAssignment = await UoW.Apartments.GetActiveManagerByUserIdAsync(resident.Id);
                if (currentAssignment != null && currentAssignment.ApartmentId == apartmentId)
                    continue;

                result.Add(new AvailableManagerDto
                {
                    UserId = resident.Id,
                    FullName = resident.FullName,
                    Email = resident.Email ?? "",
                    Phone = resident.PrimaryPhone,
                    FlatNumber = flatInApartment.Flat?.FlatNumber ?? "N/A",
                    IsCurrentlyAssigned = currentAssignment != null,
                    CurrentApartmentName = currentAssignment?.Apartment?.Name
                });
            }

            return result.OrderBy(r => r.FullName).ToList();
        }

        public async Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(
            AssignManagerRequestDto dto, Guid assignedBy)
        {
            Guid targetUserId;
            User targetUser;

            if (dto.IsExternalManager)
            {
                if (string.IsNullOrWhiteSpace(dto.ExternalManagerName))
                    throw new Exception(ManagerMessages.ManagerNameRequired);

                if (string.IsNullOrWhiteSpace(dto.ExternalManagerPhone))
                    throw new Exception(ManagerMessages.ManagerPhoneRequired);

                var existingUser = await UoW.Users.GetByPhoneAsync(dto.ExternalManagerPhone.Trim());

                if (existingUser != null)
                {
                    targetUser = existingUser;
                    targetUserId = existingUser.Id;
                }
                else
                {
                    targetUser = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = dto.ExternalManagerName.Trim(),
                        PrimaryPhone = dto.ExternalManagerPhone.Trim(),
                        Email = string.IsNullOrWhiteSpace(dto.ExternalManagerEmail)
                                                    ? null
                                                    : dto.ExternalManagerEmail.Trim(),
                        IsActive = true,
                        IsOtpVerified = true,
                        IsRegistrationCompleted = true,
                        Status = ResidentStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Stage user — UoW saves below
                    await UoW.Users.AddAsync(targetUser);
                    targetUserId = targetUser.Id;
                }
            }
            else
            {
                if (!dto.UserId.HasValue || dto.UserId == Guid.Empty)
                    throw new Exception(ResidentMessages.SelectResident);

                targetUser = await UoW.Users.GetByIdAsync(dto.UserId.Value)
                    ?? throw new Exception(ResidentMessages.ResidentNotFound);
                targetUserId = dto.UserId.Value;
            }

            var apartment = await UoW.Apartments.GetByIdAsync(dto.ApartmentId)
                ?? throw new Exception(ErrorMessages.ApartmentNotFound);

            // Deactivate existing manager of THIS apartment
            var existingManager = await UoW.Apartments.GetActiveManagerAsync(dto.ApartmentId);
            if (existingManager != null)
            {
                existingManager.IsActive = false;
                UoW.Apartments.UpdateManager(existingManager);
            }

            // Deactivate this user from any OTHER apartment
            var userOtherAssignment = await UoW.Apartments.GetActiveManagerByUserIdAsync(targetUserId);
            if (userOtherAssignment != null)
            {
                userOtherAssignment.IsActive = false;
                UoW.Apartments.UpdateManager(userOtherAssignment);
            }

            var newManager = new ApartmentManager
            {
                Id = Guid.NewGuid(),
                ApartmentId = dto.ApartmentId,
                UserId = targetUserId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await UoW.Apartments.AddManagerAsync(newManager);

            // ONE SaveChanges for user (if new) + old manager deactivation + new manager
            await UoW.SaveChangesAsync();

            return new ManagerAssignmentDto
            {
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                UserId = targetUser.Id,
                FullName = targetUser.FullName,
                Email = targetUser.Email ?? "",
                Phone = targetUser.PrimaryPhone,
                AssignedAt = newManager.AssignedAt
            };
        }

        public async Task<bool> RemoveManagerFromApartmentAsync(
            RemoveManagerRequestDto dto, Guid removedBy)
        {
            var manager = await UoW.Apartments.GetActiveManagerAsync(dto.ApartmentId)
                ?? throw new Exception(ManagerMessages.NoActiveManager);

            manager.IsActive = false;
            UoW.Apartments.UpdateManager(manager);
            await UoW.SaveChangesAsync();
            return true;
        }

        public async Task<List<ManagerListDto>> GetAllManagersAsync() =>
            new List<ManagerListDto>();

        public async Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId) =>
            null;
    }
}

*/










/*using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IUserRepository UserRepo;
        private readonly IApartmentRepository ApartmentRepo;

        public ManagerService(IUserRepository userRepository, IApartmentRepository apartmentRepository)
        {
            UserRepo = userRepository;
            ApartmentRepo = apartmentRepository;
        }

        /// Get ResidentOwners from THIS apartment
        public async Task<List<AvailableManagerDto>> GetApartmentResidentsForManagerAssignmentAsync(Guid apartmentId)
        {
            // var residentOwners = await _userRepo.GetUsersByRoleAsync("ResidentOwner");
            var residentOwners = await UserRepo.GetUsersByRoleWithFlatsAsync("ResidentOwner");

            var result = new List<AvailableManagerDto>();

            foreach (var resident in residentOwners)
            {
                // Check if this resident has a flat in THIS apartment
                var flatInApartment = resident.UserFlatMappings
                    ?.FirstOrDefault(ufm => ufm.IsActive && ufm.Flat?.ApartmentId == apartmentId);

                if (flatInApartment == null) continue;

                // Check if already manager of THIS apartment
                var currentAssignment = await ApartmentRepo.GetActiveManagerByUserIdAsync(resident.Id);
                if (currentAssignment != null && currentAssignment.ApartmentId == apartmentId)
                    continue; // Skip - already manager here

                result.Add(new AvailableManagerDto
                {
                    UserId = resident.Id,
                    FullName = resident.FullName,
                    Email = resident.Email ?? "",
                    Phone = resident.PrimaryPhone,
                    FlatNumber = flatInApartment.Flat?.FlatNumber ?? "N/A",
                    IsCurrentlyAssigned = currentAssignment != null,
                    CurrentApartmentName = currentAssignment?.Apartment?.Name
                });
            }

            return result.OrderBy(r => r.FullName).ToList();
        }

        /// Assign manager - SIMPLIFIED
        public async Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(
            AssignManagerRequestDto dto,
            Guid assignedBy)
        {
            Guid targetUserId;
            User targetUser;

            if (dto.IsExternalManager)
            {
                // EXTERNAL PERSON - Just create/find user, NO role checking

                if (string.IsNullOrWhiteSpace(dto.ExternalManagerName))
                    throw new Exception(ManagerMessages.ManagerNameRequired);

                if (string.IsNullOrWhiteSpace(dto.ExternalManagerPhone))
                    throw new Exception(ManagerMessages.ManagerPhoneRequired);

                // Check if user exists
                var existingUser = await UserRepo.GetByPhoneAsync(dto.ExternalManagerPhone.Trim());

                if (existingUser != null)
                {
                    targetUser = existingUser;
                    targetUserId = existingUser.Id;
                }
                else
                {
                    // Create new user - NO roles, just basic user
                    targetUser = new User
                    {
                        Id = Guid.NewGuid(),
                        FullName = dto.ExternalManagerName.Trim(),
                        PrimaryPhone = dto.ExternalManagerPhone.Trim(),
                        Email = string.IsNullOrWhiteSpace(dto.ExternalManagerEmail)
                            ? null
                            : dto.ExternalManagerEmail.Trim(),
                        IsActive = true,
                        IsOtpVerified = true,
                        IsRegistrationCompleted = true,
                        Status = ResidentStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };

                    await UserRepo.AddAsync(targetUser);
                    await UserRepo.SaveChangesAsync();
                    targetUserId = targetUser.Id;
                }
            }
            else
            {
                // RESIDENT FROM APARTMENT - Just use their existing account

                if (!dto.UserId.HasValue || dto.UserId == Guid.Empty)
                    throw new Exception(ResidentMessages.SelectResident);

                targetUser = await UserRepo.GetByIdAsync(dto.UserId.Value)
                    ?? throw new Exception(ResidentMessages.ResidentNotFound);

                targetUserId = dto.UserId.Value;
            }

            // ASSIGN AS APARTMENT MANAGER

            var apartment = await ApartmentRepo.GetByIdAsync(dto.ApartmentId)
                ?? throw new Exception(ErrorMessages.ApartmentNotFound);

            // Remove existing manager from THIS apartment
            var existingManager = await ApartmentRepo.GetActiveManagerAsync(dto.ApartmentId);
            if (existingManager != null)
            {
                existingManager.IsActive = false;
                await ApartmentRepo.UpdateManagerAsync(existingManager);
            }

            // Remove this user from managing any OTHER apartment
            var userOtherAssignment = await ApartmentRepo.GetActiveManagerByUserIdAsync(targetUserId);
            if (userOtherAssignment != null)
            {
                userOtherAssignment.IsActive = false;
                await ApartmentRepo.UpdateManagerAsync(userOtherAssignment);
            }

            // Create new manager assignment
            var newManager = new ApartmentManager
            {
                Id = Guid.NewGuid(),
                ApartmentId = dto.ApartmentId,
                UserId = targetUserId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await ApartmentRepo.AddManagerAsync(newManager);

            return new ManagerAssignmentDto
            {
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                UserId = targetUser.Id,
                FullName = targetUser.FullName,
                Email = targetUser.Email ?? "",
                Phone = targetUser.PrimaryPhone,
                AssignedAt = newManager.AssignedAt
            };
        }

        public async Task<bool> RemoveManagerFromApartmentAsync(RemoveManagerRequestDto dto, Guid removedBy)
        {
            var manager = await ApartmentRepo.GetActiveManagerAsync(dto.ApartmentId)
                ?? throw new Exception(ManagerMessages.NoActiveManager);

            manager.IsActive = false;
            await ApartmentRepo.UpdateManagerAsync(manager);
            return true;
        }

        public async Task<List<ManagerListDto>> GetAllManagersAsync()
        {
            // Implementation not needed for this feature
            return new List<ManagerListDto>();
        }

        public async Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId)
        {
            // Implementation not needed for this feature
            return null;
        }
    }
}
*/







