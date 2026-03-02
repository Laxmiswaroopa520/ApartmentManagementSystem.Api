using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for managing staff members within the community.
    /// 
    /// Handles:
    /// - Retrieving staff members
    /// - Creating new staff members
    /// - Updating staff details
    /// - Activating and deactivating staff accounts
    /// </summary>
    public class StaffMemberService : IStaffMemberService
    {
        /// <summary>
        /// Repository responsible for staff member data access.
        /// </summary>
        private readonly IStaffMemberRepository StaffMemberRepository;

        /// <summary>
        /// Constructor for injecting staff member repository dependency.
        /// </summary>
        /// <param name="repository">
        /// Repository that handles staff member database operations.
        /// </param>
        public StaffMemberService(IStaffMemberRepository repository)
        {
            StaffMemberRepository = repository;
        }

        /// <summary>
        /// Retrieves all staff members.
        /// </summary>
        /// <returns>
        /// List of staff members.
        /// </returns>
        public async Task<List<StaffMemberDto>> GetAllStaffMembersAsync()
        {
            return await StaffMemberRepository.GetAllAsync();
        }

        /// <summary>
        /// Retrieves staff members filtered by staff type.
        /// </summary>
        /// <param name="staffType">
        /// Type of staff (e.g., Security, Housekeeping, Maintenance).
        /// </param>
        /// <returns>
        /// List of staff members matching the specified type.
        /// </returns>
        public async Task<List<StaffMemberDto>> GetStaffMembersByTypeAsync(string staffType)
        {
            return await StaffMemberRepository.GetByTypeAsync(staffType);
        }

        /// <summary>
        /// Retrieves a specific staff member by unique identifier.
        /// </summary>
        /// <param name="staffId">
        /// Unique identifier of the staff member.
        /// </param>
        /// <returns>
        /// Staff member details if found; otherwise null.
        /// </returns>
        public async Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid staffId)
        {
            return await StaffMemberRepository.GetByIdAsync(staffId);
        }

        /// <summary>
        /// Creates a new staff member.
        /// 
        /// Validations:
        /// - Staff type must be valid
        /// - Phone number must be unique
        /// </summary>
        /// <param name="dto">
        /// Staff member creation details.
        /// </param>
        /// <param name="createdBy">
        /// User ID of the person creating the staff member.
        /// </param>
        /// <returns>
        /// Newly created staff member details.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown when staff type is invalid or phone number already exists.
        /// </exception>
        public async Task<StaffMemberDto> CreateStaffMemberAsync(
            CreateStaffMemberDto dto, Guid createdBy)
        {
            // Validate staff type
            if (!RoleNames.GetStaffRoles().Contains(dto.StaffType))
                throw new Exception($"Invalid staff type: {dto.StaffType}");

            // Ensure phone number uniqueness
            var phoneExists = await StaffMemberRepository.PhoneExistsAsync(dto.Phone);
            if (phoneExists)
                throw new Exception("Staff member with this phone number already exists");

            // Create staff member
            await StaffMemberRepository.CreateAsync(dto, createdBy);

            // Retrieve newly created staff member
            var staff = await StaffMemberRepository.GetByTypeAsync(dto.StaffType);
            return staff.First(s => s.Phone == dto.Phone);
        }

        /// <summary>
        /// Updates an existing staff member's details.
        /// </summary>
        /// <param name="dto">
        /// Staff member update details.
        /// </param>
        /// <param name="updatedBy">
        /// User ID of the person performing the update.
        /// </param>
        /// <returns>
        /// Updated staff member details.
        /// </returns>
        public async Task<StaffMemberDto> UpdateStaffMemberAsync(
            UpdateStaffMemberDto dto, Guid updatedBy)
        {
            await StaffMemberRepository.UpdateAsync(dto, updatedBy);

            return (await StaffMemberRepository.GetByIdAsync(dto.StaffId))!;
        }

        /// <summary>
        /// Deactivates a staff member.
        /// </summary>
        /// <param name="staffId">
        /// Unique identifier of the staff member.
        /// </param>
        /// <param name="deactivatedBy">
        /// User ID performing the deactivation.
        /// </param>
        /// <returns>
        /// True if deactivation is successful.
        /// </returns>
        public async Task<bool> DeactivateStaffMemberAsync(Guid staffId, Guid deactivatedBy)
        {
            await StaffMemberRepository.SetActiveStatusAsync(staffId, false, deactivatedBy);
            return true;
        }

        /// <summary>
        /// Activates a previously deactivated staff member.
        /// </summary>
        /// <param name="staffId">
        /// Unique identifier of the staff member.
        /// </param>
        /// <param name="activatedBy">
        /// User ID performing the activation.
        /// </param>
        /// <returns>
        /// True if activation is successful.
        /// </returns>
        public async Task<bool> ActivateStaffMemberAsync(Guid staffId, Guid activatedBy)
        {
            await StaffMemberRepository.SetActiveStatusAsync(staffId, true, activatedBy);
            return true;
        }
    }
}




























/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class StaffMemberService : IStaffMemberService
    {
        private readonly IStaffMemberRepository StaffMemberRepository;

        public StaffMemberService(IStaffMemberRepository repository)
        {
            StaffMemberRepository = repository;
        }

        public async Task<List<StaffMemberDto>> GetAllStaffMembersAsync()
        {
            return await StaffMemberRepository.GetAllAsync();
        }

        public async Task<List<StaffMemberDto>> GetStaffMembersByTypeAsync(string staffType)
        {
            return await StaffMemberRepository.GetByTypeAsync(staffType);
        }

        public async Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid staffId)
        {
            return await StaffMemberRepository.GetByIdAsync(staffId);
        }

        public async Task<StaffMemberDto> CreateStaffMemberAsync(
            CreateStaffMemberDto dto, Guid createdBy)
        {
            if (!RoleNames.GetStaffRoles().Contains(dto.StaffType))
                throw new Exception($"Invalid staff type: {dto.StaffType}");

            var phoneExists = await StaffMemberRepository.PhoneExistsAsync(dto.Phone);
            if (phoneExists)
                throw new Exception("Staff member with this phone number already exists");

            await StaffMemberRepository.CreateAsync(dto, createdBy);

            var staff = await StaffMemberRepository.GetByTypeAsync(dto.StaffType);
            return staff.First(s => s.Phone == dto.Phone);
        }

        public async Task<StaffMemberDto> UpdateStaffMemberAsync(
            UpdateStaffMemberDto dto, Guid updatedBy)
        {
            await StaffMemberRepository.UpdateAsync(dto, updatedBy);

            return (await StaffMemberRepository.GetByIdAsync(dto.StaffId))!;
        }

        public async Task<bool> DeactivateStaffMemberAsync(Guid staffId, Guid deactivatedBy)
        {
            await StaffMemberRepository.SetActiveStatusAsync(staffId, false, deactivatedBy);
            return true;
        }

        public async Task<bool> ActivateStaffMemberAsync(Guid staffId, Guid activatedBy)
        {
            await StaffMemberRepository.SetActiveStatusAsync(staffId, true, activatedBy);
            return true;
        }
    }
}
*/
