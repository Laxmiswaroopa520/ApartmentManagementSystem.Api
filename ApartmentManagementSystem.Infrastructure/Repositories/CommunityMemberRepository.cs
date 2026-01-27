// Infrastructure/Repositories/CommunityMemberRepository.cs
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











/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
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
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur =>
                    RoleNames.GetCommunityRoles().Contains(ur.Role.Name)))
                .Select(u => new CommunityMemberDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = u.UserRoles
                        .Where(ur => RoleNames.GetCommunityRoles().Contains(ur.Role.Name))
                        .Select(ur => ur.Role.Name)
                        .First(),
                    AssignedOn = u.CreatedAt,
                    IsActive = true
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ResidentListDto>> GetEligibleResidentsAsync()
        {
            return await _context.Users
                .Where(u =>
                    u.UserRoles.Any(ur => ur.Role.Name == RoleNames.ResidentOwner) &&
                    u.UserFlatMappings.Any())
                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    ResidentType = "Owner",
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .First(),
                    Status = u.UserRoles.Any(ur =>
                        RoleNames.GetCommunityRoles().Contains(ur.Role.Name))
                        ? "Has Community Role"
                        : "Eligible",
                    RegisteredOn = u.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new CommunityMemberDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = u.UserRoles
                        .Where(ur => RoleNames.GetCommunityRoles().Contains(ur.Role.Name))
                        .Select(ur => ur.Role.Name)
                        .FirstOrDefault(),
                    AssignedOn = u.CreatedAt,
                    IsActive = true
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CommunityRoleExistsAsync(string roleName)
        {
            return await _context.Users
                .AnyAsync(u => u.UserRoles.Any(ur => ur.Role.Name == roleName));
        }

        public async Task AssignCommunityRoleAsync(Guid userId, string roleName)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstAsync(u => u.Id == userId);

            var role = await _context.Roles
                .FirstAsync(r => r.Name == roleName);

            user.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = role.Id
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstAsync(u => u.Id == userId);

            var userRole = user.UserRoles
                .First(ur => RoleNames.GetCommunityRoles().Contains(ur.Role.Name));

            user.UserRoles.Remove(userRole);

            await _context.SaveChangesAsync();
        }
    }
}

*/















/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Constants;
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
            return await _context.Users
                // .Where(u => u.Roles.Any(r => RoleNames.GetCommunityRoles().Contains(r.Name)))
                .Where(u => u.UserRoles.Any(ur =>
    RoleNames.GetCommunityRoles().Contains(ur.Role.Name)))

                .Select(u => new CommunityMemberDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    //  Role = u.Roles
                    //    .Where(r => RoleNames.GetCommunityRoles().Contains(r.Name))
                    //  .Select(r => r.Name)
                    //.First(),
                    Role = u.UserRoles
    .Where(ur => RoleNames.GetCommunityRoles().Contains(ur.Role.Name))
    .Select(ur => ur.Role.Name)
    .First(),

                    AssignedOn = u.CreatedAt,
                    IsActive = true
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ResidentListDto>> GetEligibleResidentsAsync()
        {
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == RoleNames.ResidentOwner)
                         && u.UserFlatMappings.Any())
                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    ResidentType = "Owner",
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .First(),
                    Status = u.UserRoles.Any(ur => RoleNames.GetCommunityRoles().Contains(ur.Role.Name))
                        ? "Has Community Role"
                        : "Eligible",
                    RegisteredOn = u.CreatedAt
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CommunityMemberDto?> GetCommunityMemberByUserIdAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new CommunityMemberDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = u.UserRoles
                        .Where(r => RoleNames.GetCommunityRoles().Contains(r.Role.Name))
                        .Select(r => r.Role.Name)
                        .FirstOrDefault()!,
                    AssignedOn = u.CreatedAt,
                    IsActive = true
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CommunityRoleExistsAsync(string roleName)
        {
            return await _context.Users
                .AnyAsync(u => u.UserRoles.Any(r => r.Role.Name == roleName));
        }

        public async Task AssignCommunityRoleAsync(Guid userId, string roleName)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstAsync(u => u.Id == userId);

            var role = await _context.Roles.FirstAsync(r => r.Name == roleName);
            // user.Roles.Add(role);//Updated One
            user.UserRoles.Add(new UserRole
            {
                RoleId = role.Id
            });


            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstAsync(u => u.Id == userId);

            var role = user.UserRoles
             //   .First(r => RoleNames.GetCommunityRoles().Contains(r.Name));
             .First(ur => RoleNames.GetCommunityRoles().Contains(ur.Role.Name));

            //  user.UserRoles.Remove(role); updated one..
            var ur = user.UserRoles
      .First(ur => RoleNames.GetCommunityRoles().Contains(ur.Role.Name));

            user.UserRoles.Remove(ur);

            await _context.SaveChangesAsync();
        }
    }
}
*/