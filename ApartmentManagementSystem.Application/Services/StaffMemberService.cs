using ApartmentManagementSystem.Application.DTOs.Community;
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
