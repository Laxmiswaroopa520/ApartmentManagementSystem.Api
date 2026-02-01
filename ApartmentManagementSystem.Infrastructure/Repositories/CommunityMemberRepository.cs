// Infrastructure/Repositories/CommunityMemberRepository.cs























/*

using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class CommunityMemberRepository : ICommunityMemberRepository
    {
        private readonly AppDbContext _context;

        public CommunityMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync()
        {
            return await _context.Set<CommunityMember>()
                .Include(cm => cm.User)
                    .ThenInclude(u => u.UserFlatMappings)
                        .ThenInclude(ufm => ufm.Flat)
                .Where(cm => cm.IsActive)
                .Select(cm => new CommunityMemberDto
                {
                    UserId = cm.UserId,
                    FullName = cm.User.FullName,
                    Email = cm.User.Email ?? "",
                    Phone = cm.User.PrimaryPhone,
                    FlatNumber = cm.User.UserFlatMappings
                        .Where(ufm => ufm.IsActive)
                        .Select(ufm => ufm.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = cm.CommunityRole,
                    AssignedOn = cm.AssignedAt,
                    IsActive = cm.IsActive
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ResidentListDto>> GetEligibleResidentsAsync()
        {
            // Get users who are resident owners with assigned flats
            // but don't already have a community role
            var usersWithCommunityRoles = await _context.Set<CommunityMember>()
                .Where(cm => cm.IsActive)
                .Select(cm => cm.UserId)
                .ToListAsync();

            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.UserFlatMappings)
                    .ThenInclude(ufm => ufm.Flat)
                .Where(u =>
                    u.UserRoles.Any(ur => ur.Role.Name == "ResidentOwner") &&
                    u.UserFlatMappings.Any(ufm => ufm.IsActive) &&
                    !usersWithCommunityRoles.Contains(u.Id))
                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Phone = u.PrimaryPhone,
                    ResidentType = "Owner",
                    FlatNumber = u.UserFlatMappings
                        .Where(ufm => ufm.IsActive)
                        .Select(ufm => ufm.Flat.FlatNumber)
                        .First(),
                    Status = "Eligible",
                    RegisteredOn = u.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await _context.Set<CommunityMember>()
                .Include(cm => cm.User)
                    .ThenInclude(u => u.UserFlatMappings)
                        .ThenInclude(ufm => ufm.Flat)
                .Where(cm => cm.UserId == userId && cm.IsActive)
                .Select(cm => new CommunityMemberDto
                {
                    UserId = cm.UserId,
                    FullName = cm.User.FullName,
                    Email = cm.User.Email ?? "",
                    Phone = cm.User.PrimaryPhone,
                    FlatNumber = cm.User.UserFlatMappings
                        .Where(ufm => ufm.IsActive)
                        .Select(ufm => ufm.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = cm.CommunityRole,
                    AssignedOn = cm.AssignedAt,
                    IsActive = cm.IsActive
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CommunityRoleExistsAsync(string roleName)
        {
            return await _context.Set<CommunityMember>()
                .AnyAsync(cm => cm.CommunityRole == roleName && cm.IsActive);
        }

        public async Task AssignCommunityRoleAsync(Guid userId, string roleName)
        {
            var user = await _context.Users
                .Include(u => u.UserFlatMappings)
                    .ThenInclude(ufm => ufm.Flat)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            var flatMapping = user.UserFlatMappings?.FirstOrDefault(ufm => ufm.IsActive);
            if (flatMapping?.Flat == null)
                throw new Exception("User must have an assigned flat");

            var communityMember = new CommunityMember
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ApartmentId = flatMapping.Flat.ApartmentId,
                CommunityRole = roleName,
                AssignedBy = Guid.Empty, // Will be set by service
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Set<CommunityMember>().AddAsync(communityMember);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            var communityMember = await _context.Set<CommunityMember>()
                .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.IsActive);

            if (communityMember == null)
                throw new Exception("Community member not found");

            communityMember.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
*/








       

using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class CommunityMemberRepository : ICommunityMemberRepository
    {
        private readonly AppDbContext _context;

        public CommunityMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        // ─── GET ALL (no filter) ──────────────────────────────────────────────
        public async Task<List<CommunityMemberDto>> GetAllCommunityMembersAsync()
        {
            return await _context.Set<CommunityMember>()
                .Include(cm => cm.User)
                    .ThenInclude(u => u.UserFlatMappings)
                        .ThenInclude(ufm => ufm.Flat)
                .Where(cm => cm.IsActive)
                .Select(cm => new CommunityMemberDto
                {
                    UserId = cm.UserId,
                    FullName = cm.User.FullName,
                    Email = cm.User.Email ?? "",
                    Phone = cm.User.PrimaryPhone,
                    ApartmentId = cm.ApartmentId,   // ⭐ include for filtering in service
                    FlatNumber = cm.User.UserFlatMappings
                        .Where(ufm => ufm.IsActive)
                        .Select(ufm => ufm.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = cm.CommunityRole,
                    AssignedOn = cm.AssignedAt,
                    IsActive = cm.IsActive
                })
                .AsNoTracking()
                .ToListAsync();
        }

        // ─── ELIGIBLE RESIDENTS (legacy, no apartment filter) ────────────────
        public async Task<List<ResidentListDto>> GetEligibleResidentsAsync()
        {
            var usersWithRoles = await _context.Set<CommunityMember>()
                .Where(cm => cm.IsActive)
                .Select(cm => cm.UserId)
                .ToListAsync();

            return await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserFlatMappings).ThenInclude(ufm => ufm.Flat)
                .Where(u =>
                    u.UserRoles.Any(ur => ur.Role.Name == "ResidentOwner") &&
                    u.UserFlatMappings.Any(ufm => ufm.IsActive) &&
                    !usersWithRoles.Contains(u.Id))
                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Phone = u.PrimaryPhone,
                    ResidentType = "Owner",
                    FlatNumber = u.UserFlatMappings
                        .Where(ufm => ufm.IsActive)
                        .Select(ufm => ufm.Flat.FlatNumber)
                        .First(),
                    Status = "Eligible",
                    RegisteredOn = u.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();
        }

        // ─── ⭐ ELIGIBLE RESIDENTS FOR A SPECIFIC APARTMENT ──────────────────
        public async Task<List<ResidentListDto>> GetEligibleResidentsForApartmentAsync(Guid apartmentId)
        {
            // Get users who ALREADY have a community role in THIS apartment
            var usersWithRolesInApartment = await _context.Set<CommunityMember>()
                .Where(cm => cm.IsActive && cm.ApartmentId == apartmentId)
                .Select(cm => cm.UserId)
                .ToListAsync();

            // Return resident owners who:
            //   1) Have an active flat in THIS apartment
            //   2) Don't already have a community role in THIS apartment
            return await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.UserFlatMappings).ThenInclude(ufm => ufm.Flat)
                .Where(u =>
                    u.UserRoles.Any(ur => ur.Role.Name == "ResidentOwner") &&
                    u.UserFlatMappings.Any(ufm => ufm.IsActive && ufm.Flat.ApartmentId == apartmentId) &&
                    !usersWithRolesInApartment.Contains(u.Id))
                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Phone = u.PrimaryPhone,
                    ResidentType = "Owner",
                    FlatNumber = u.UserFlatMappings
                        .Where(ufm => ufm.IsActive && ufm.Flat.ApartmentId == apartmentId)
                        .Select(ufm => ufm.Flat.FlatNumber)
                        .First(),
                    Status = "Eligible",
                    RegisteredOn = u.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();
        }

        // ─── GET SINGLE MEMBER ────────────────────────────────────────────────
        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await _context.Set<CommunityMember>()
                .Include(cm => cm.User)
                    .ThenInclude(u => u.UserFlatMappings)
                        .ThenInclude(ufm => ufm.Flat)
                .Where(cm => cm.UserId == userId && cm.IsActive)
                .Select(cm => new CommunityMemberDto
                {
                    UserId = cm.UserId,
                    FullName = cm.User.FullName,
                    Email = cm.User.Email ?? "",
                    Phone = cm.User.PrimaryPhone,
                    ApartmentId = cm.ApartmentId,
                    FlatNumber = cm.User.UserFlatMappings
                        .Where(ufm => ufm.IsActive)
                        .Select(ufm => ufm.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = cm.CommunityRole,
                    AssignedOn = cm.AssignedAt,
                    IsActive = cm.IsActive
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        // ─── ROLE EXISTS (global) ─────────────────────────────────────────────
        public async Task<bool> CommunityRoleExistsAsync(string roleName)
        {
            return await _context.Set<CommunityMember>()
                .AnyAsync(cm => cm.CommunityRole == roleName && cm.IsActive);
        }

        // ─── ⭐ ROLE EXISTS (scoped to apartment) ────────────────────────────
        public async Task<bool> CommunityRoleExistsForApartmentAsync(string roleName, Guid apartmentId)
        {
            return await _context.Set<CommunityMember>()
                .AnyAsync(cm => cm.CommunityRole == roleName && cm.ApartmentId == apartmentId && cm.IsActive);
        }

        // ─── ⭐ ASSIGN ROLE (updated signature) ──────────────────────────────
        public async Task AssignCommunityRoleAsync(Guid userId, string roleName, Guid apartmentId, Guid assignedBy)
        {
            var communityMember = new CommunityMember
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ApartmentId = apartmentId,
                CommunityRole = roleName,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Set<CommunityMember>().AddAsync(communityMember);
            await _context.SaveChangesAsync();
        }

        // ─── REMOVE ROLE ──────────────────────────────────────────────────────
        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            var communityMember = await _context.Set<CommunityMember>()
                .FirstOrDefaultAsync(cm => cm.UserId == userId && cm.IsActive);

            if (communityMember == null)
                throw new Exception("Community member not found");

            communityMember.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}