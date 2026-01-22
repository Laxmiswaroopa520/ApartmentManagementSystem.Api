using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Application.Services
{
    public class StaffMemberService : IStaffMemberService
    {
        private readonly IStaffMemberRepository _repository;

        public StaffMemberService(IStaffMemberRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StaffMemberDto>> GetAllStaffMembersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<StaffMemberDto>> GetStaffMembersByTypeAsync(string staffType)
        {
            return await _repository.GetByTypeAsync(staffType);
        }

        public async Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid staffId)
        {
            return await _repository.GetByIdAsync(staffId);
        }

        public async Task<StaffMemberDto> CreateStaffMemberAsync(
            CreateStaffMemberDto dto, Guid createdBy)
        {
            if (!RoleNames.GetStaffRoles().Contains(dto.StaffType))
                throw new Exception($"Invalid staff type: {dto.StaffType}");

            var phoneExists = await _repository.PhoneExistsAsync(dto.Phone);
            if (phoneExists)
                throw new Exception("Staff member with this phone number already exists");

            await _repository.CreateAsync(dto, createdBy);

            var staff = await _repository.GetByTypeAsync(dto.StaffType);
            return staff.First(s => s.Phone == dto.Phone);
        }

        public async Task<StaffMemberDto> UpdateStaffMemberAsync(
            UpdateStaffMemberDto dto, Guid updatedBy)
        {
            await _repository.UpdateAsync(dto, updatedBy);

            return (await _repository.GetByIdAsync(dto.StaffId))!;
        }

        public async Task<bool> DeactivateStaffMemberAsync(Guid staffId, Guid deactivatedBy)
        {
            await _repository.SetActiveStatusAsync(staffId, false, deactivatedBy);
            return true;
        }

        public async Task<bool> ActivateStaffMemberAsync(Guid staffId, Guid activatedBy)
        {
            await _repository.SetActiveStatusAsync(staffId, true, activatedBy);
            return true;
        }
    }
}
