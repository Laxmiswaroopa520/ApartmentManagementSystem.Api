using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
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
