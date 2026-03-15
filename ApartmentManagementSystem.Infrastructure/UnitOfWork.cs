using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApartmentManagementSystem.Infrastructure
{
    /// <summary>
    /// Unit of Work implementation.
    /// All repositories share ONE AppDbContext instance.
    /// SaveChangesAsync() is called ONCE here — never inside any repository.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private IDbContextTransaction? _transaction;

        // ── Nullable backing fields ────────────────────────────────
        // These MUST be nullable (?) so they start as null and are assigned
        // on first access via ??=. Declaring them as non-nullable was causing:
        //   "Non-nullable field must contain a non-null value"
        //   "Possible null reference assignment"
        private IApartmentRepository? _apartments;
        private IFlatRepository? _flats;
        private IFloorRepository? _floors;
        private IUserRepository? _users;
        private IRoleRepository? _roles;
        private IUserFlatMappingRepository? _userFlatMappings;
        private IUserInviteRepository? _userInvites;
        private IUserOtpRepository? _userOtps;
        private ICommunityMemberRepository? _communityMembers;
        private IStaffMemberRepository? _staffMembers;
        private IResidentManagementRepository? _residents;
        private IEnhancedDashboardRepository? _dashboard;

        public UnitOfWork(AppDbContext db, IPasswordHasher passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        // ── Lazy-init properties ───────────────────────────────────
        // Pattern: backing field ?? new Repo(). Created only on first access.
        // All share the same _db instance so EF tracks everything together.

        public IApartmentRepository Apartments
            => _apartments ??= new ApartmentRepository(_db);

        public IFlatRepository Flats
            => _flats ??= new FlatRepository(_db);

        public IFloorRepository Floors
            => _floors ??= new FloorRepository(_db);

        public IUserRepository Users
            => _users ??= new UserRepository(_db);

        public IRoleRepository Roles
            => _roles ??= new RoleRepository(_db);

        public IUserFlatMappingRepository UserFlatMappings
            => _userFlatMappings ??= new UserFlatMappingRepository(_db);

        public IUserInviteRepository UserInvites
            => _userInvites ??= new UserInviteRepository(_db);

        public IUserOtpRepository UserOtps
            => _userOtps ??= new UserOtpRepository(_db);

        public ICommunityMemberRepository CommunityMembers
            => _communityMembers ??= new CommunityMemberRepository(_db);

        public IStaffMemberRepository StaffMembers
            => _staffMembers ??= new StaffMemberRepository(
                _db,
                Users,           // reuses already-initialised UserRepository
                Roles,           // reuses already-initialised RoleRepository
                _passwordHasher);

        public IResidentManagementRepository Residents
            => _residents ??= new ResidentManagementRepository(_db);

        public IEnhancedDashboardRepository Dashboard
            => _dashboard ??= new EnhancedDashboardRepository(_db);

        // ── Single SaveChanges ─────────────────────────────────────
        /// <summary>
        /// Commits ALL staged changes across ALL repositories in one DB call.
        /// </summary>
        public async Task<int> SaveChangesAsync()
            => await _db.SaveChangesAsync();

        // ── Transaction support ────────────────────────────────────
        public async Task BeginTransactionAsync()
            => _transaction = await _db.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _db.Dispose();
        }
    }
}












/*using ApartmentManagementSystem.Application.Interfaces;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Infrastructure.Persistence;
using ApartmentManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApartmentManagementSystem.Infrastructure
{
    /// <summary>
    /// Unit of Work implementation.
    /// - Lazily initialises repositories (only created when first accessed).
    /// - All repositories share the SAME AppDbContext instance so EF change
    ///   tracking works across them without extra round-trips.
    /// - SaveChangesAsync() is called ONCE here — never inside a repository.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext db;

        // Dependencies needed by specific repositories
        private readonly IUserRepository UserRepoInstance;
        private readonly IRoleRepository RoleRepoInstance;
        private readonly IPasswordHasher PasswordHasher;

        private IDbContextTransaction? _Transaction;

        // ── Lazy backing fields ────────────────────────────────────
        private IApartmentRepository? _Apartments;
        private IFlatRepository? _Flats;
        private IFloorRepository? _Floors;
        private IUserFlatMappingRepository? _UserFlatMappings;
        private IUserInviteRepository? _UserInvites;
        private IUserOtpRepository? _UserOtps;
        private ICommunityMemberRepository? _CommunityMembers;
        private IStaffMemberRepository? _StaffMembers;
        private IResidentManagementRepository? _Residents;
        private IEnhancedDashboardRepository? _Dashboard;

        public UnitOfWork(
            AppDbContext db,
            IPasswordHasher passwordHasher)
        {
            this.db = db;
            PasswordHasher = passwordHasher;

            // These two are needed inside other repos so instantiate directly
            UserRepoInstance = new UserRepository(db);
            RoleRepoInstance = new RoleRepository(db);
        }

        // ── Repository Properties (lazy init) ─────────────────────
        public IApartmentRepository Apartments
            => Apartments ??= new ApartmentRepository(db);

        public IFlatRepository Flats
            => Flats ??= new FlatRepository(db);

        public IFloorRepository Floors
            => Floors ??= new FloorRepository(db);

        public IUserRepository Users
            => UserRepoInstance;

        public IRoleRepository Roles
            => RoleRepoInstance;

        public IUserFlatMappingRepository UserFlatMappings
            => UserFlatMappings ??= new UserFlatMappingRepository(db);

        public IUserInviteRepository UserInvites
            => UserInvites ??= new UserInviteRepository(db);

        public IUserOtpRepository UserOtps
            => UserOtps ??= new UserOtpRepository(db);

        public ICommunityMemberRepository CommunityMembers
            => CommunityMembers ??= new CommunityMemberRepository(db);

        public IStaffMemberRepository StaffMembers
            => StaffMembers ??= new StaffMemberRepository(
                db, UserRepoInstance, RoleRepoInstance, PasswordHasher);

        public IResidentManagementRepository Residents
            => Residents ??= new ResidentManagementRepository(db);

        public IEnhancedDashboardRepository Dashboard
            => Dashboard ??= new EnhancedDashboardRepository(db);

        // ── Single SaveChanges ─────────────────────────────────────
        /// <summary>
        /// Commits ALL pending changes across ALL repositories in one DB call.
        /// Services call this once after completing their full operation.
        /// </summary>
        public async Task<int> SaveChangesAsync()
            => await db.SaveChangesAsync();

        // ── Transaction support ────────────────────────────────────
        public async Task BeginTransactionAsync()
            => _Transaction = await db.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            if (_Transaction != null)
            {
                await _Transaction.CommitAsync();
                await _Transaction.DisposeAsync();
                _Transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_Transaction != null)
            {
                await _Transaction.RollbackAsync();
                await _Transaction.DisposeAsync();
                _Transaction = null;
            }
        }

        public void Dispose()
        {
            _Transaction?.Dispose();
            db.Dispose();
        }
    }
}
*/