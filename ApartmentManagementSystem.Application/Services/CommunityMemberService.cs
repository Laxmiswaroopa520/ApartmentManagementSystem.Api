using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.Application.Services
{
    public class CommunityMemberService : ICommunityMemberService
    {
        private readonly IUnitOfWork UoW;

        public CommunityMemberService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync(Guid? apartmentId = null)
        {
            var all = await UoW.CommunityMembers.GetAllCommunityMembersAsync();

            if (apartmentId.HasValue)
                all = all.Where(m => m.ApartmentId == apartmentId.Value).ToList();

            return all;
        }

        public async Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId)
            => await UoW.CommunityMembers.GetEligibleResidentsForApartmentAsync(apartmentId);

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
            => await UoW.CommunityMembers.GetCommunityMemberByUserIdAsync(userId);

        public async Task<CommunityMemberDto> AssignCommunityRoleAsync(
            Guid userId, string roleName, Guid apartmentId, Guid assignedBy)
        {
            var roleExists = await UoW.CommunityMembers
                .CommunityRoleExistsForApartmentAsync(roleName, apartmentId);

            if (roleExists)
                throw new Exception(
                    $"The {roleName} role is already assigned in this apartment. Remove the existing one first.");

            // Stage the new community member
            await UoW.CommunityMembers.AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy);

            // ONE SaveChanges
            await UoW.SaveChangesAsync();

            var member = await UoW.CommunityMembers.GetCommunityMemberByUserIdAsync(userId)
                ?? throw new Exception(CommunityMessages.NoCommunityMember);

            return member;
        }

        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            // Mutates in memory
            await UoW.CommunityMembers.RemoveCommunityRoleAsync(userId);

            // ONE SaveChanges
            await UoW.SaveChangesAsync();
        }
    }
}



















/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Constants;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for managing community member operations.
    /// 
    /// Handles:
    /// - Retrieving community members
    /// - Filtering members by apartment
    /// - Identifying eligible residents for community roles
    /// - Assigning and removing community roles
    /// </summary>
    public class CommunityMemberService : ICommunityMemberService
    {
        /// <summary>
        /// Repository used for community member data access.
        /// </summary>
        private readonly ICommunityMemberRepository CommunityMemberRepo;

        /// <summary>
        /// Constructor for injecting community member repository dependency.
        /// </summary>
        /// <param name="communityMemberRepository">
        /// Repository responsible for community member-related database operations.
        /// </param>
        public CommunityMemberService(ICommunityMemberRepository communityMemberRepository)
        {
            CommunityMemberRepo = communityMemberRepository;
        }

        /// <summary>
        /// Retrieves all community members.
        /// If an apartment ID is provided, results are filtered to that apartment.
        /// Otherwise, all community members are returned.
        /// </summary>
        /// <param name="apartmentId">
        /// Optional apartment identifier used to filter community members.
        /// </param>
        /// <returns>
        /// List of community members.
        /// </returns>
        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync(Guid? apartmentId = null)
        {
            var all = await CommunityMemberRepo.GetAllCommunityMembersAsync();

            if (apartmentId.HasValue)
            {
                // Filter members belonging to the specified apartment
                all = all
                    .Where(m => m.ApartmentId == apartmentId.Value)
                    .ToList();
            }

            return all;
        }

        /// <summary>
        /// Retrieves resident owners who:
        /// 1. Have an active flat in the specified apartment.
        /// 2. Do NOT already have a community role in that apartment.
        /// </summary>
        /// <param name="apartmentId">
        /// Unique identifier of the apartment.
        /// </param>
        /// <returns>
        /// List of eligible residents for community role assignment.
        /// </returns>
        public async Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId)
        {
            return await CommunityMemberRepo
                .GetEligibleResidentsForApartmentAsync(apartmentId);
        }

        /// <summary>
        /// Retrieves community member details by user ID.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the user.
        /// </param>
        /// <returns>
        /// Community member details if found; otherwise null.
        /// </returns>
        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await CommunityMemberRepo
                .GetCommunityMemberByUserIdAsync(userId);
        }

        /// <summary>
        /// Assigns a community role to a resident within a specific apartment.
        /// 
        /// Validations:
        /// - Ensures the role is not already assigned within the apartment.
        /// - Repository handles validation that the user is an eligible resident.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the resident.
        /// </param>
        /// <param name="roleName">
        /// Name of the community role to assign.
        /// </param>
        /// <param name="apartmentId">
        /// Unique identifier of the apartment.
        /// </param>
        /// <param name="assignedBy">
        /// User ID of the person assigning the role.
        /// </param>
        /// <returns>
        /// The newly assigned community member details.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when:
        /// - The role already exists in the apartment.
        /// - The assigned member cannot be retrieved after assignment.
        /// </exception>
        public async Task<CommunityMemberDto> AssignCommunityRoleAsync(
            Guid userId,
            string roleName,
            Guid apartmentId,
            Guid assignedBy)
        {
            // Check if the role already exists in the apartment
            var roleExists = await CommunityMemberRepo
                .CommunityRoleExistsForApartmentAsync(roleName, apartmentId);

            if (roleExists)
                throw new Exception(
                    $"The {roleName} role is already assigned in this apartment. Remove the existing one first."
                );

            // Assign the role
            await CommunityMemberRepo
                .AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy);

            // Retrieve newly assigned member
            var member = await CommunityMemberRepo
                .GetCommunityMemberByUserIdAsync(userId);

            if (member == null)
                throw new Exception(CommunityMessages.NoCommunityMember);

            return member;
        }

        /// <summary>
        /// Removes a community role from a user.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the user whose role should be removed.
        /// </param>
        /// <returns>
        /// Task representing the asynchronous operation.
        /// </returns>
        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            await CommunityMemberRepo.RemoveCommunityRoleAsync(userId);
        }
    }
}
*/













