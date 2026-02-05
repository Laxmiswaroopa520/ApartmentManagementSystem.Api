using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
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

        /// <summary>
        /// Get ResidentOwners from THIS apartment
        /// </summary>
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

        /// <summary>
        /// Assign manager - SIMPLIFIED
        /// </summary>
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
                    throw new Exception("Manager name is required");

                if (string.IsNullOrWhiteSpace(dto.ExternalManagerPhone))
                    throw new Exception("Manager phone is required");

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
                    throw new Exception("Please select a resident");

                targetUser = await UserRepo.GetByIdAsync(dto.UserId.Value)
                    ?? throw new Exception("Resident not found");

                targetUserId = dto.UserId.Value;
            }

            // ASSIGN AS APARTMENT MANAGER

            var apartment = await ApartmentRepo.GetByIdAsync(dto.ApartmentId)
                ?? throw new Exception("Apartment not found");

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
                ?? throw new Exception("No active manager found");

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









/*using ApartmentManagementSystem.Application.DTOs.Manager;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;

// Application/Services/ManagerService.cs
using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IUserRepository _userRepo;
        private readonly IApartmentRepository _apartmentRepo;

        public ManagerService(
            IUserRepository userRepository,
            IApartmentRepository apartmentRepository)
        {
            _userRepo = userRepository;
            _apartmentRepo = apartmentRepository;
        }

        public async Task<List<AvailableManagerDto>> GetAvailableManagersAsync(Guid apartmentId)
        {
            // Get all users with Manager role
            var managers = await _userRepo.GetUsersByRoleAsync("Manager");

            var availableManagers = new List<AvailableManagerDto>();

            foreach (var manager in managers)
            {
                // Check if manager is currently assigned to any apartment
                var currentAssignment = await _apartmentRepo.GetActiveManagerByUserIdAsync(manager.Id);

                // Exclude manager if already assigned to THIS apartment
                if (currentAssignment != null && currentAssignment.ApartmentId == apartmentId)
                {
                    continue;
                }

                availableManagers.Add(new AvailableManagerDto
                {
                    UserId = manager.Id,
                    FullName = manager.FullName,
                    Email = manager.Email ?? "",
                    Phone = manager.PrimaryPhone,
                    IsCurrentlyAssigned = currentAssignment != null,
                    CurrentApartmentName = currentAssignment?.Apartment?.Name
                });
            }

            return availableManagers.OrderBy(m => m.FullName).ToList();
        }

        public async Task<ManagerAssignmentDto> AssignManagerToApartmentAsync(
            AssignManagerRequestDto dto,
            Guid assignedBy)
        {
            // Validate user exists and has Manager role
            var user = await _userRepo.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception(ErrorMessages.UserNotFound);

            var hasManagerRole = user.UserRoles?.Any(ur => ur.Role.Name == "Manager") ?? false;
            if (!hasManagerRole)
                throw new Exception("User must have Manager role to be assigned as apartment manager");

            // Validate apartment exists
            var apartment = await _apartmentRepo.GetByIdAsync(dto.ApartmentId);
            if (apartment == null)
                throw new Exception("Apartment not found");

            // Remove any existing manager from this apartment
            var existingManager = await _apartmentRepo.GetActiveManagerAsync(dto.ApartmentId);
            if (existingManager != null)
            {
                existingManager.IsActive = false;
                await _apartmentRepo.UpdateManagerAsync(existingManager);
            }

            // Remove this user from any other apartment they're managing
            var userCurrentAssignment = await _apartmentRepo.GetActiveManagerByUserIdAsync(dto.UserId);
            if (userCurrentAssignment != null)
            {
                userCurrentAssignment.IsActive = false;
                await _apartmentRepo.UpdateManagerAsync(userCurrentAssignment);
            }

            // Create new manager assignment
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

            return new ManagerAssignmentDto
            {
                ApartmentId = apartment.Id,
                ApartmentName = apartment.Name,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Phone = user.PrimaryPhone,
                AssignedAt = newManager.AssignedAt
            };
        }

        public async Task<bool> RemoveManagerFromApartmentAsync(
            RemoveManagerRequestDto dto,
            Guid removedBy)
        {
            var manager = await _apartmentRepo.GetActiveManagerAsync(dto.ApartmentId);
            if (manager == null)
                throw new Exception("No active manager found for this apartment");

            manager.IsActive = false;
            await _apartmentRepo.UpdateManagerAsync(manager);

            return true;
        }

        public async Task<List<ManagerListDto>> GetAllManagersAsync()
        {
            var managers = await _userRepo.GetUsersByRoleAsync("Manager");

            var managerList = new List<ManagerListDto>();

            foreach (var manager in managers)
            {
                var assignment = await _apartmentRepo.GetActiveManagerByUserIdAsync(manager.Id);

                managerList.Add(new ManagerListDto
                {
                    UserId = manager.Id,
                    FullName = manager.FullName,
                    Email = manager.Email ?? "",
                    Phone = manager.PrimaryPhone,
                    ApartmentId = assignment?.ApartmentId,
                    ApartmentName = assignment?.Apartment?.Name,
                    IsActive = assignment != null,
                    AssignedAt = assignment?.AssignedAt
                });
            }

            return managerList.OrderBy(m => m.FullName).ToList();
        }

        public async Task<ManagerListDto?> GetManagerByUserIdAsync(Guid userId)
        {
            var manager = await _userRepo.GetByIdAsync(userId);
            if (manager == null) return null;

            var assignment = await _apartmentRepo.GetActiveManagerByUserIdAsync(userId);

            return new ManagerListDto
            {
                UserId = manager.Id,
                FullName = manager.FullName,
                Email = manager.Email ?? "",
                Phone = manager.PrimaryPhone,
                ApartmentId = assignment?.ApartmentId,
                ApartmentName = assignment?.Apartment?.Name,
                IsActive = assignment != null,
                AssignedAt = assignment?.AssignedAt
            };
        }
    }
}

*/