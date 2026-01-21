using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface ICommunityMemberService
    {
        Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync();
        Task<List<ResidentListDto>> GetEligibleResidentsForCommunityRoleAsync();
        Task<CommunityMemberDto> AssignCommunityRoleAsync(AssignCommunityRoleDto dto, Guid assignedBy);
        Task<bool> RemoveCommunityRoleAsync(RemoveCommunityRoleDto dto, Guid removedBy);
        Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId);
    }

}
