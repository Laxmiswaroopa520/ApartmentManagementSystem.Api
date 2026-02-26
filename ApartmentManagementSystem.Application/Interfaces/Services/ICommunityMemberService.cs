using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
        public interface ICommunityMemberService
        {
            //  Now accepts optional apartmentId to filter</summary>
            Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync(Guid? apartmentId = null);

            // Returns eligible residents for a SPECIFIC apartment only</summary>
            Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId);
            Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId);
            //  Now requires apartmentId</summary>
            Task<CommunityMemberDto> AssignCommunityRoleAsync(Guid userId, string roleName, Guid apartmentId, Guid assignedBy);
            Task RemoveCommunityRoleAsync(Guid userId);
        }
    }


