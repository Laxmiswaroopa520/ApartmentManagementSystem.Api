using ApartmentManagementSystem.Application.DTOs.Community;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IStaffMemberRepository
    {
        Task<List<StaffMemberDto>> GetAllAsync();
        Task<List<StaffMemberDto>> GetByTypeAsync(string staffType);
        Task<StaffMemberDto?> GetByIdAsync(Guid staffId);
        Task<bool> PhoneExistsAsync(string phone);
        Task PrepareCreateAsync(CreateStaffMemberDto dto, Guid createdBy); // stages, UoW saves
        Task PrepareUpdateAsync(UpdateStaffMemberDto dto, Guid updatedBy); // stages, UoW saves
        Task PrepareSetActiveStatusAsync(Guid staffId, bool isActive, Guid updatedBy);
    }
}





/*using ApartmentManagementSystem.Application.DTOs.Community;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IStaffMemberRepository
    {
        Task<List<StaffMemberDto>> GetAllAsync();
        Task<List<StaffMemberDto>> GetByTypeAsync(string staffType);
        Task<StaffMemberDto?> GetByIdAsync(Guid staffId);

        Task<bool> PhoneExistsAsync(string phone);
        Task CreateAsync(CreateStaffMemberDto dto, Guid createdBy);
        Task UpdateAsync(UpdateStaffMemberDto dto, Guid updatedBy);

        Task SetActiveStatusAsync(Guid staffId, bool isActive, Guid updatedBy);
    }
}
*/