using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation.
    /// All domain repositories inherit from this — they get CRUD for free
    /// and only need to add their own domain-specific queries on top.
    /// NOTE: No SaveChangesAsync here. UnitOfWork owns that responsibility.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext DBContext;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(AppDbContext context)
        {
            DBContext = context;
            DbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id)
            => await DbSet.FindAsync(id);

        public async Task<List<T>> GetAllAsync()
            => await DbSet.ToListAsync();

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await DbSet.Where(predicate).ToListAsync();

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await DbSet.FirstOrDefaultAsync(predicate);

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await DbSet.AnyAsync(predicate);

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
            => predicate == null
                ? await DbSet.CountAsync()
                : await DbSet.CountAsync(predicate);

        public async Task AddAsync(T entity)
            => await DbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await DbSet.AddRangeAsync(entities);

        public void Update(T entity)
            => DbSet.Update(entity);

        public void Remove(T entity)
            => DbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities)
            => DbSet.RemoveRange(entities);
    }
}