using ApartmentManagementSystem.Application.DTOs.Community;
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
