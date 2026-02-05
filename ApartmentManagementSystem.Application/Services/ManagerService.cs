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








