using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{

    using ApartmentManagementSystem.Application.DTOs.Community;
    using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;

   
        public interface ICommunityMemberService
        {
            ///  Now accepts optional apartmentId to filter</summary>
            Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync(Guid? apartmentId = null);

            ///  NEW: Returns eligible residents for a SPECIFIC apartment only</summary>
            Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId);

            Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId);

            ///  Now requires apartmentId</summary>
            Task<CommunityMemberDto> AssignCommunityRoleAsync(Guid userId, string roleName, Guid apartmentId, Guid assignedBy);

            Task RemoveCommunityRoleAsync(Guid userId);
        }
    }


