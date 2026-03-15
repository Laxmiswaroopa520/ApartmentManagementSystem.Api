using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class StaffMemberService : IStaffMemberService
    {
        private readonly IUnitOfWork UoW;

        public StaffMemberService(IUnitOfWork unitOfWork)
        {
            UoW = unitOfWork;
        }

        public Task<List<StaffMemberDto>> GetAllStaffMembersAsync() =>
            UoW.StaffMembers.GetAllAsync();

        public Task<List<StaffMemberDto>> GetStaffMembersByTypeAsync(string staffType) =>
            UoW.StaffMembers.GetByTypeAsync(staffType);

        public Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid staffId) =>
            UoW.StaffMembers.GetByIdAsync(staffId);

        public async Task<StaffMemberDto> CreateStaffMemberAsync(
            CreateStaffMemberDto dto, Guid createdBy)
        {
            if (!RoleNames.GetStaffRoles().Contains(dto.StaffType))
                throw new Exception($"Invalid staff type: {dto.StaffType}");

            if (await UoW.StaffMembers.PhoneExistsAsync(dto.Phone))
                throw new Exception("Staff member with this phone number already exists.");

            // Stage everything in memory
            await UoW.StaffMembers.PrepareCreateAsync(dto, createdBy);

            // ONE SaveChanges
            await UoW.SaveChangesAsync();

            var all = await UoW.StaffMembers.GetByTypeAsync(dto.StaffType);
            return all.First(s => s.Phone == dto.Phone);
        }

        public async Task<StaffMemberDto> UpdateStaffMemberAsync(
            UpdateStaffMemberDto dto, Guid updatedBy)
        {
            // Stage update in memory
            await UoW.StaffMembers.PrepareUpdateAsync(dto, updatedBy);

            // ONE SaveChanges
            await UoW.SaveChangesAsync();

            return (await UoW.StaffMembers.GetByIdAsync(dto.StaffId))!;
        }

        public async Task<bool> DeactivateStaffMemberAsync(Guid staffId, Guid deactivatedBy)
        {
            await UoW.StaffMembers.PrepareSetActiveStatusAsync(staffId, false, deactivatedBy);
            await UoW.SaveChangesAsync();
            return true;
        }

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













































