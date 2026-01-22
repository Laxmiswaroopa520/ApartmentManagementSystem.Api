using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class CommunityMemberService : ICommunityMemberService
    {
        private readonly ICommunityMemberRepository _repository;

        public CommunityMemberService(ICommunityMemberRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync()
        {
            return await _repository.GetAllCommunityMembersAsync();
        }

        public async Task<List<ResidentListDto>> GetEligibleResidentsForCommunityRoleAsync()
        {
            return await _repository.GetEligibleResidentsAsync();
        }

        public async Task<CommunityMemberDto> AssignCommunityRoleAsync(
            AssignCommunityRoleDto dto, Guid assignedBy)
        {
            if (!RoleNames.GetCommunityRoles().Contains(dto.CommunityRole))
                throw new Exception("Invalid community role");

            var exists = await _repository.CommunityRoleExistsAsync(dto.CommunityRole);
            if (exists)
                throw new Exception($"{dto.CommunityRole} role already assigned");

            await _repository.AssignCommunityRoleAsync(dto.UserId, dto.CommunityRole);

            var member = await _repository.GetCommunityMemberByUserIdAsync(dto.UserId);
            return member!;
        }

        public async Task<bool> RemoveCommunityRoleAsync(
            RemoveCommunityRoleDto dto, Guid removedBy)
        {
            await _repository.RemoveCommunityRoleAsync(dto.UserId);
            return true;
        }

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await _repository.GetCommunityMemberByUserIdAsync(userId);
        }
    }
}
