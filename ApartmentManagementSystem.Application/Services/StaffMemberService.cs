using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    /// <summary>
    /// Service responsible for staff member management operations.
    ///
    /// Handles:
    /// - Listing all staff or filtering by staff type
    /// - Creating staff members with optional system user account creation
    /// - Updating staff member details
    /// - Activating and deactivating staff member accounts
    ///
    /// All write operations use the Prepare pattern — changes are staged in memory
    /// via the repository and committed with a single UoW.SaveChangesAsync() call.
    /// </summary>
    public class StaffMemberService : IStaffMemberService
    {
        /// <summary>Unit of Work providing access to all repositories.</summary>
        private readonly IUnitOfWork UoW;

        /// <summary>
        /// Initialises StaffMemberService with required dependencies.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for data access.</param>
        public StaffMemberService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        /// <summary>
        /// Retrieves all staff members ordered by join date descending.
        /// </summary>
        /// <returns>List of staff member DTOs.</returns>
        public Task<List<StaffMemberDto>> GetAllStaffMembersAsync() =>
            UoW.StaffMembers.GetAllAsync();

        /// <summary>
        /// Retrieves staff members filtered by staff type (e.g. Security, Plumber).
        /// </summary>
        /// <param name="staffType">Staff type string to filter by.</param>
        /// <returns>List of staff member DTOs matching the type.</returns>
        public Task<List<StaffMemberDto>> GetStaffMembersByTypeAsync(string staffType) =>
            UoW.StaffMembers.GetByTypeAsync(staffType);

        /// <summary>
        /// Retrieves a single staff member by their staff record ID.
        /// </summary>
        /// <param name="staffId">Unique identifier of the staff record.</param>
        /// <returns>Staff member DTO or null if not found.</returns>
        public Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid staffId) =>
            UoW.StaffMembers.GetByIdAsync(staffId);

        /// <summary>
        /// Creates a new staff member record.
        ///
        /// Validations:
        /// - Staff type must be a recognised role from the configured staff roles list.
        /// - Phone number must not already exist.
        ///
        /// If a password is provided, a system user account is also created and linked.
        /// All records are staged and committed in one SaveChanges.
        /// </summary>
        /// <param name="dto">Staff member creation data.</param>
        /// <param name="createdBy">UserId of the admin performing the creation.</param>
        /// <returns>The newly created staff member DTO.</returns>
        /// <exception cref="Exception">Thrown on invalid staff type or duplicate phone number.</exception>
        public async Task<StaffMemberDto> CreateStaffMemberAsync(
            CreateStaffMemberDto dto, Guid createdBy)
        {
            if (!RoleNames.GetStaffRoles().Contains(dto.StaffType))
                throw new Exception($"Invalid staff type: {dto.StaffType}");

            if (await UoW.StaffMembers.PhoneExistsAsync(dto.Phone))
                throw new Exception("Staff member with this phone number already exists.");

            await UoW.StaffMembers.PrepareCreateAsync(dto, createdBy);
            await UoW.SaveChangesAsync();

            var all = await UoW.StaffMembers.GetByTypeAsync(dto.StaffType);
            return all.First(s => s.Phone == dto.Phone);
        }

        /// <summary>
        /// Updates an existing staff member's details.
        ///
        /// If the staff member has a linked system user account,
        /// that account's name, phone, and email are also updated.
        /// Changes are staged and committed in one SaveChanges.
        /// </summary>
        /// <param name="dto">Updated staff member values.</param>
        /// <param name="updatedBy">UserId of the admin performing the update.</param>
        /// <returns>The updated staff member DTO.</returns>
        public async Task<StaffMemberDto> UpdateStaffMemberAsync(
            UpdateStaffMemberDto dto, Guid updatedBy)
        {
            await UoW.StaffMembers.PrepareUpdateAsync(dto, updatedBy);
            await UoW.SaveChangesAsync();
            return (await UoW.StaffMembers.GetByIdAsync(dto.StaffId))!;
        }

        /// <summary>
        /// Deactivates a staff member account.
        /// The record is retained; IsActive is set to false.
        /// </summary>
        /// <param name="staffId">Unique identifier of the staff record to deactivate.</param>
        /// <param name="deactivatedBy">UserId of the admin performing the deactivation.</param>
        /// <returns>True on success.</returns>
        public async Task<bool> DeactivateStaffMemberAsync(Guid staffId, Guid deactivatedBy)
        {
            await UoW.StaffMembers.PrepareSetActiveStatusAsync(staffId, false, deactivatedBy);
            await UoW.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Re-activates a previously deactivated staff member account.
        /// </summary>
        /// <param name="staffId">Unique identifier of the staff record to activate.</param>
        /// <param name="activatedBy">UserId of the admin performing the activation.</param>
        /// <returns>True on success.</returns>
        public async Task<bool> ActivateStaffMemberAsync(Guid staffId, Guid activatedBy)
        {
            await UoW.StaffMembers.PrepareSetActiveStatusAsync(staffId, true, activatedBy);
            await UoW.SaveChangesAsync();
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
    /// <summary>
    /// Manages staff members; now supports apartment-scoped assignment.
    /// </summary>
    public class StaffMemberService : IStaffMemberService
    {
        private readonly IStaffMemberRepository StaffMemberRepository;

        public StaffMemberService(IStaffMemberRepository repository)
        {
            StaffMemberRepository = repository;
        }

        public Task<List<StaffMemberDto>> GetAllStaffMembersAsync() =>
            StaffMemberRepository.GetAllAsync();

        public Task<List<StaffMemberDto>> GetStaffMembersByTypeAsync(string staffType) =>
            StaffMemberRepository.GetByTypeAsync(staffType);

        public Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid staffId) =>
            StaffMemberRepository.GetByIdAsync(staffId);

        public async Task<StaffMemberDto> CreateStaffMemberAsync(
            CreateStaffMemberDto dto, Guid createdBy)
        {
            if (!RoleNames.GetStaffRoles().Contains(dto.StaffType))
                throw new Exception($"Invalid staff type: {dto.StaffType}");

            if (await StaffMemberRepository.PhoneExistsAsync(dto.Phone))
                throw new Exception("Staff member with this phone number already exists");

            await StaffMemberRepository.CreateAsync(dto, createdBy);

            var all = await StaffMemberRepository.GetByTypeAsync(dto.StaffType);
            return all.First(s => s.Phone == dto.Phone);
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













































