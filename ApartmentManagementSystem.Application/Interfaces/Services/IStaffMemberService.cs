using ApartmentManagementSystem.Application.DTOs.Community;
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IStaffMemberService
    {
        Task<List<StaffMemberDto>> GetAllStaffMembersAsync();
        Task<List<StaffMemberDto>> GetStaffMembersByTypeAsync(string staffType);
        Task<StaffMemberDto> CreateStaffMemberAsync(CreateStaffMemberDto dto, Guid createdBy);
        Task<StaffMemberDto> UpdateStaffMemberAsync(UpdateStaffMemberDto dto, Guid updatedBy);
        Task<bool> DeactivateStaffMemberAsync(Guid staffId, Guid deactivatedBy);
        Task<bool> ActivateStaffMemberAsync(Guid staffId, Guid activatedBy);
        Task<StaffMemberDto?> GetStaffMemberByIdAsync(Guid staffId);
    }
}
