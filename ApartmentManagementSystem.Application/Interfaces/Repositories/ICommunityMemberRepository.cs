
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface ICommunityMemberRepository
    {
        Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync();
        Task<List<ResidentListDto>> GetEligibleResidentsAsync();

        /// <summary>⭐ NEW: Eligible residents filtered to a specific apartment</summary>
        Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId);

        Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId);

        Task<bool> CommunityRoleExistsAsync(string roleName);

        /// <summary>⭐ NEW: Check role existence scoped to an apartment</summary>
        Task<bool> CommunityRoleExistsForApartmentAsync(string roleName, Guid apartmentId);

        /// <summary>⭐ UPDATED signature: now accepts apartmentId and assignedBy</summary>
        Task AssignCommunityRoleAsync(Guid userId, string roleName, Guid apartmentId, Guid assignedBy);

        Task RemoveCommunityRoleAsync(Guid userId);
    }
}


















/*
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface ICommunityMemberRepository
    {
        Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync();
        Task<List<ResidentListDto>> GetEligibleResidentsAsync();
        Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId);
        Task<bool> CommunityRoleExistsAsync(string roleName);
        Task AssignCommunityRoleAsync(Guid userId, string roleName);
        Task RemoveCommunityRoleAsync(Guid userId);
    }
}
*/