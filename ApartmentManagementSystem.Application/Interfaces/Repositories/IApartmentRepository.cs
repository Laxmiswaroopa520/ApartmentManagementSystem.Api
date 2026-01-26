using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IApartmentRepository
    {
        Task<Apartment?> GetByIdAsync(Guid id);
        Task<Apartment?> GetByIdWithFloorsAndFlatsAsync(Guid id);
        Task<Apartment?> GetByIdWithFullDetailsAsync(Guid id);
        Task<List<Apartment>> GetAllWithDetailsAsync();
        Task<List<Apartment>> GetAllAsync();
        Task<int> GetTotalCountAsync();
        Task<ApartmentManager?> GetActiveManagerAsync(Guid apartmentId);
        Task<ApartmentManager?> GetActiveManagerByUserIdAsync(Guid userId); // ⭐ NEW
        Task AddAsync(Apartment apartment);
        Task UpdateAsync(Apartment apartment);
        Task AddManagerAsync(ApartmentManager manager);
        Task UpdateManagerAsync(ApartmentManager manager);
        Task SaveChangesAsync();
    }
}
/*
using ApartmentManagementSystem.Domain.Entities;

namespace ApartmentManagementSystem.Application.Interfaces.Repositories
{
    public interface IApartmentRepository
    {
        Task<Apartment?> GetByIdAsync(Guid id);
        Task<Apartment?> GetByIdWithFloorsAndFlatsAsync(Guid id);
        Task<Apartment?> GetByIdWithFullDetailsAsync(Guid id);
        Task<List<Apartment>> GetAllWithDetailsAsync();
        Task<List<Apartment>> GetAllAsync();
        Task<int> GetTotalCountAsync();
        Task<ApartmentManager?> GetActiveManagerAsync(Guid apartmentId);
        Task AddAsync(Apartment apartment);
        Task UpdateAsync(Apartment apartment);
        Task AddManagerAsync(ApartmentManager manager);
        Task UpdateManagerAsync(ApartmentManager manager);
        Task SaveChangesAsync();
    }
}
*/