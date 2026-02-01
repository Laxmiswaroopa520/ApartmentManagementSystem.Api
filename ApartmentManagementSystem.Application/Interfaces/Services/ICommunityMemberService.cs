using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    /*  public interface ICommunityMemberService
      {
          Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync();
          Task<List<ResidentListDto>> GetEligibleResidentsForCommunityRoleAsync();
          Task<CommunityMemberDto> AssignCommunityRoleAsync(AssignCommunityRoleDto dto, Guid assignedBy);
          Task<bool> RemoveCommunityRoleAsync(RemoveCommunityRoleDto dto, Guid removedBy);
          Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId);
      }*/
    // Application/Interfaces/Services/ICommunityMemberService.cs
    // COMPLETE REPLACEMENT

    using ApartmentManagementSystem.Application.DTOs.Community;
    using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;

   
        public interface ICommunityMemberService
        {
            /// <summary>⭐ Now accepts optional apartmentId to filter</summary>
            Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync(Guid? apartmentId = null);

            /// <summary>⭐ NEW: Returns eligible residents for a SPECIFIC apartment only</summary>
            Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId);

            Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId);

            /// <summary>⭐ Now requires apartmentId</summary>
            Task<CommunityMemberDto> AssignCommunityRoleAsync(Guid userId, string roleName, Guid apartmentId, Guid assignedBy);

            Task RemoveCommunityRoleAsync(Guid userId);
        }
    }


