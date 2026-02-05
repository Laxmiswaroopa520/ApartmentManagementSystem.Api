using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Application.Services
{
    public class CommunityMemberService : ICommunityMemberService
    {
        private readonly ICommunityMemberRepository CommunityMemberRepo;

        public CommunityMemberService(ICommunityMemberRepository communityMemberRepository)
        {
            CommunityMemberRepo = communityMemberRepository;
        }

        /// <summary>
        /// ⭐ If apartmentId is provided, filter to that apartment only.
        /// Otherwise return all (backward-compatible).
        /// </summary>
        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync(Guid? apartmentId = null)
        {
            var all = await CommunityMemberRepo.GetAllCommunityMembersAsync();

            if (apartmentId.HasValue)
            {
                // Filter: only members whose flat belongs to this apartment
                // Note: CommunityMember has ApartmentId directly, so filter on that
                all = all.Where(m => m.ApartmentId == apartmentId.Value).ToList();
            }

            return all;
        }

        /// <summary>
        /// ⭐ NEW: Returns resident owners who:
        ///   1) Have an active flat in THIS apartment
        ///   2) Do NOT already have a community role in THIS apartment
        /// </summary>
        public async Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId)
        {
            return await CommunityMemberRepo.GetEligibleResidentsForApartmentAsync(apartmentId);
        }

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await CommunityMemberRepo.GetCommunityMemberByUserIdAsync(userId);
        }

        /// <summary>
        /// ⭐ Validates that the user is a resident owner in the given apartment before assigning.
        /// </summary>
        public async Task<CommunityMemberDto> AssignCommunityRoleAsync(
            Guid userId, string roleName, Guid apartmentId, Guid assignedBy)
        {
            // Check if this role already exists in this apartment
            var roleExists = await CommunityMemberRepo.CommunityRoleExistsForApartmentAsync(roleName, apartmentId);
            if (roleExists)
                throw new Exception($"The {roleName} role is already assigned in this apartment. Remove the existing one first.");

            // Assign the role (repository handles creating the CommunityMember record)
            await CommunityMemberRepo.AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy);

            // Return the newly created member DTO
            var member = await CommunityMemberRepo.GetCommunityMemberByUserIdAsync(userId);
            if (member == null)
                throw new Exception("Failed to retrieve the assigned community member.");

            return member;
        }

        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            await CommunityMemberRepo.RemoveCommunityRoleAsync(userId);
        }
    }
}






























