// Application/Services/CommunityMemberService.cs
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class CommunityMemberService : ICommunityMemberService
    {
        private readonly ICommunityMemberRepository _communityRepo;
        private readonly IUserRepository _userRepo;
        private readonly IApartmentRepository _apartmentRepo;

        public CommunityMemberService(
            ICommunityMemberRepository communityMemberRepository,
            IUserRepository userRepository,
            IApartmentRepository apartmentRepository)
        {
            _communityRepo = communityMemberRepository;
            _userRepo = userRepository;
            _apartmentRepo = apartmentRepository;
        }

        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync()
        {
            return await _communityRepo.GetAllCommunityMembersAsync();
        }

        public async Task<List<ResidentListDto>> GetEligibleResidentsForCommunityRoleAsync()
        {
            var eligibleResidents = await _communityRepo.GetEligibleResidentsAsync();

            // Add display text for dropdown
            foreach (var resident in eligibleResidents)
            {
                resident.DisplayText = $"{resident.FullName} - Flat {resident.FlatNumber}";
            }

            return eligibleResidents;
        }

        public async Task<CommunityMemberDto> AssignCommunityRoleAsync(
            AssignCommunityRoleDto dto,
            Guid assignedBy)
        {
            // Validate user exists
            var user = await _userRepo.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new Exception(ErrorMessages.UserNotFound);

            // Validate role is a valid community role
            var validRoles = new[] { "President", "Secretary", "Treasurer" };
            if (!validRoles.Contains(dto.CommunityRole))
                throw new Exception("Invalid community role");

            // Check if user is a resident owner with flat assignment
            var hasOwnerRole = user.UserRoles?.Any(ur => ur.Role.Name == RoleNames.ResidentOwner) ?? false;
            if (!hasOwnerRole)
                throw new Exception("Only resident owners can be assigned community roles");

            var hasFlat = user.UserFlatMappings?.Any(ufm => ufm.IsActive) ?? false;
            if (!hasFlat)
                throw new Exception("User must have an assigned flat to hold a community role");

            // Check if role is already taken
            var roleExists = await _communityRepo.CommunityRoleExistsAsync(dto.CommunityRole);
            if (roleExists)
                throw new Exception($"{dto.CommunityRole} role is already assigned to another member");

            // Get apartment ID from user's flat
            var flatMapping = user.UserFlatMappings?.FirstOrDefault(ufm => ufm.IsActive);
            var flat = flatMapping?.Flat;
            if (flat == null)
                throw new Exception("Could not determine user's apartment");

            // Create community member entry
            var communityMember = new CommunityMember
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ApartmentId = flat.ApartmentId,
                CommunityRole = dto.CommunityRole,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Save to database
            await _communityRepo.AssignCommunityRoleAsync(dto.UserId, dto.CommunityRole);

            // Return the created member details
            var member = await _communityRepo.GetCommunityMemberByUserIdAsync(dto.UserId);
            if (member == null)
                throw new Exception("Failed to retrieve assigned community member");

            return member;
        }

        public async Task<bool> RemoveCommunityRoleAsync(
            RemoveCommunityRoleDto dto,
            Guid removedBy)
        {
            var member = await _communityRepo.GetCommunityMemberByUserIdAsync(dto.UserId);
            if (member == null)
                throw new Exception("Community member not found");

            await _communityRepo.RemoveCommunityRoleAsync(dto.UserId);
            return true;
        }

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await _communityRepo.GetCommunityMemberByUserIdAsync(userId);
        }
    }
}












/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class CommunityMemberService : ICommunityMemberService
    {
        private readonly ICommunityMemberRepository Communityrepository;

        public CommunityMemberService(ICommunityMemberRepository repository)
        {
            Communityrepository = repository;
        }

        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync()
        {
            return await Communityrepository.GetAllCommunityMembersAsync();
        }

        public async Task<List<ResidentListDto>> GetEligibleResidentsForCommunityRoleAsync()
        {
            return await Communityrepository.GetEligibleResidentsAsync();
        }

        public async Task<CommunityMemberDto> AssignCommunityRoleAsync(
            AssignCommunityRoleDto dto, Guid assignedBy)
        {
            if (!RoleNames.GetCommunityRoles().Contains(dto.CommunityRole))
                throw new Exception("Invalid community role");

            var exists = await Communityrepository.CommunityRoleExistsAsync(dto.CommunityRole);
            if (exists)
                throw new Exception($"{dto.CommunityRole} role already assigned");

            await Communityrepository.AssignCommunityRoleAsync(dto.UserId, dto.CommunityRole);

            var member = await Communityrepository.GetCommunityMemberByUserIdAsync(dto.UserId);
            return member!;
        }

        public async Task<bool> RemoveCommunityRoleAsync(
            RemoveCommunityRoleDto dto, Guid removedBy)
        {
            await Communityrepository.RemoveCommunityRoleAsync(dto.UserId);
            return true;
        }

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await Communityrepository.GetCommunityMemberByUserIdAsync(userId);
        }
    }
}
*/