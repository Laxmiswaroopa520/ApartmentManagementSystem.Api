using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly AppDbContext DBContext;

        public ApartmentRepository(AppDbContext context)
        {
            DBContext = context;
        }

        public async Task<Apartment?> GetByIdAsync(Guid id)
            => await DBContext.Apartments.FindAsync(id);

        public async Task<Apartment?> GetByIdWithFloorsAndFlatsAsync(Guid id)
            => await DBContext.Apartments
                .Include(a => a.Floors)
                    .ThenInclude(f => f.Flats)
                        .ThenInclude(flat => flat.UserFlatMappings!)
                            .ThenInclude(ufm => ufm.User)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Apartment?> GetByIdWithFullDetailsAsync(Guid id)
            => await DBContext.Apartments
                .Include(a => a.Floors)
                .Include(a => a.Flats)
                .Include(a => a.Managers.Where(m => m.IsActive))
                    .ThenInclude(m => m.User)
                .Include(a => a.CommunityMembers.Where(cm => cm.IsActive))
                    .ThenInclude(cm => cm.User)
                        .ThenInclude(u => u.UserFlatMappings!)
                            .ThenInclude(ufm => ufm.Flat)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task<List<Apartment>> GetAllWithDetailsAsync()
            => await DBContext.Apartments
                .Include(a => a.Flats)
                .Include(a => a.Managers.Where(m => m.IsActive))
                .Where(a => a.IsActive)
                .ToListAsync();

        public async Task<List<Apartment>> GetAllAsync()
            => await DBContext.Apartments
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();

        public async Task<int> GetTotalCountAsync()
            => await DBContext.Apartments.CountAsync(a => a.IsActive);

        public async Task<ApartmentManager?> GetActiveManagerAsync(Guid apartmentId)
            => await DBContext.Set<ApartmentManager>()
                .Include(m => m.User)
                .Include(m => m.Apartment)
                .FirstOrDefaultAsync(m => m.ApartmentId == apartmentId && m.IsActive);

        public async Task<ApartmentManager?> GetActiveManagerByUserIdAsync(Guid userId)
            => await DBContext.Set<ApartmentManager>()
                .Include(m => m.Apartment)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);

        public async Task AddAsync(Apartment apartment)
        {
            await DBContext.Apartments.AddAsync(apartment);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Apartment apartment)
        {
            DBContext.Apartments.Update(apartment);
            await DBContext.SaveChangesAsync();
        }

        //  hard-delete apartment with all children
        public async Task DeleteAsync(Apartment apartment)
        {
            // Load all related data so EF can cascade-delete
            var full = await DBContext.Apartments
                .Include(a => a.Floors)
                    .ThenInclude(f => f.Flats)
                        .ThenInclude(flat => flat.UserFlatMappings!)
                .Include(a => a.Managers)
                .Include(a => a.CommunityMembers)
                .FirstOrDefaultAsync(a => a.Id == apartment.Id);

            if (full == null) return;

            // Remove UserFlatMappings first (FK constraint)
            foreach (var floor in full.Floors)
                foreach (var flat in floor.Flats)
                    DBContext.UserFlatMappings.RemoveRange(flat.UserFlatMappings ?? new List<UserFlatMapping>());

            // Remove Flats
            foreach (var floor in full.Floors)
                DBContext.Flats.RemoveRange(floor.Flats);

            // Remove Floors
            DBContext.Floors.RemoveRange(full.Floors);

            // Remove Managers & Community
            DBContext.Set<ApartmentManager>().RemoveRange(full.Managers);
            DBContext.Set<CommunityMember>().RemoveRange(full.CommunityMembers);

            // Remove Apartment
            DBContext.Apartments.Remove(full);

            await DBContext.SaveChangesAsync();
        }

        public async Task AddManagerAsync(ApartmentManager manager)
        {
            await DBContext.Set<ApartmentManager>().AddAsync(manager);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateManagerAsync(ApartmentManager manager)
        {
            DBContext.Set<ApartmentManager>().Update(manager);
            await DBContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
            => await DBContext.SaveChangesAsync();
    }
}


















/*using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly AppDbContext DBContext;

        public ApartmentRepository(AppDbContext context)
        {
            DBContext = context;
        }

        public async Task<Apartment?> GetByIdAsync(Guid id)
        {
            return await DBContext.Apartments.FindAsync(id);
        }

        public async Task<Apartment?> GetByIdWithFloorsAndFlatsAsync(Guid id)
        {
            return await DBContext.Apartments
                .Include(a => a.Floors)
                    .ThenInclude(f => f.Flats)
                        .ThenInclude(flat => flat.UserFlatMappings!)
                            .ThenInclude(ufm => ufm.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Apartment?> GetByIdWithFullDetailsAsync(Guid id)
        {
            return await DBContext.Apartments
                .Include(a => a.Floors)
                .Include(a => a.Flats)
                .Include(a => a.Managers.Where(m => m.IsActive))
                    .ThenInclude(m => m.User)
                .Include(a => a.CommunityMembers.Where(cm => cm.IsActive))
                    .ThenInclude(cm => cm.User)
                        .ThenInclude(u => u.UserFlatMappings!)
                            .ThenInclude(ufm => ufm.Flat)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Apartment>> GetAllWithDetailsAsync()
        {
            return await DBContext.Apartments
                .Include(a => a.Flats)
                .Include(a => a.Managers.Where(m => m.IsActive))
                .Where(a => a.IsActive)
                .ToListAsync();
        }

        public async Task<List<Apartment>> GetAllAsync()
        {
            return await DBContext.Apartments
                .Where(a => a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await DBContext.Apartments.CountAsync(a => a.IsActive);
        }

        public async Task<ApartmentManager?> GetActiveManagerAsync(Guid apartmentId)
        {
            return await DBContext.Set<ApartmentManager>()
                .Include(m => m.User)
                .Include(m => m.Apartment)
                .FirstOrDefaultAsync(m => m.ApartmentId == apartmentId && m.IsActive);
        }

        // ⭐ NEW: Get manager by user ID
        public async Task<ApartmentManager?> GetActiveManagerByUserIdAsync(Guid userId)
        {
            return await DBContext.Set<ApartmentManager>()
                .Include(m => m.Apartment)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive);
        }

        public async Task AddAsync(Apartment apartment)
        {
            await DBContext.Apartments.AddAsync(apartment);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Apartment apartment)
        {
            DBContext.Apartments.Update(apartment);
            await DBContext.SaveChangesAsync();
        }

        public async Task AddManagerAsync(ApartmentManager manager)
        {
            await DBContext.Set<ApartmentManager>().AddAsync(manager);
            await DBContext.SaveChangesAsync();
        }

        public async Task UpdateManagerAsync(ApartmentManager manager)
        {
            DBContext.Set<ApartmentManager>().Update(manager);
            await DBContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await DBContext.SaveChangesAsync();
        }
    }
}
*/