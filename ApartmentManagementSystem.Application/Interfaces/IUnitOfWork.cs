using ApartmentManagementSystem.Application.Interfaces.Repositories;

namespace ApartmentManagementSystem.Application.Interfaces
{
    /// <summary>
    /// Unit of Work interface.
    /// Coordinates all repository operations and ensures a single
    /// SaveChangesAsync() call commits everything atomically.
    /// Services inject IUnitOfWork instead of individual repositories.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // ── Repositories ──────────────────────────────────────────
        IApartmentRepository Apartments { get; }
        IFlatRepository Flats { get; }
        IFloorRepository Floors { get; }
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IUserFlatMappingRepository UserFlatMappings { get; }
        IUserInviteRepository UserInvites { get; }
        IUserOtpRepository UserOtps { get; }
        ICommunityMemberRepository CommunityMembers { get; }
        IStaffMemberRepository StaffMembers { get; }
        IResidentManagementRepository Residents { get; }
        IEnhancedDashboardRepository Dashboard { get; }

        // ── Persistence ───────────────────────────────────────────
        /// <summary>
        /// Commits all pending changes across all repositories in one DB call.
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Begins a database transaction for multi-step atomic operations.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current transaction on failure.
        /// </summary>
        Task RollbackTransactionAsync();
    }
}