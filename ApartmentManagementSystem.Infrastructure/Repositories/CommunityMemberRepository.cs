using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Constants;
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
                .Where(u => u.Roles.Any(r => UserRole.GetCommunityRoles().Contains(r.Name)))
                .Select(u => new CommunityMemberDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault() ?? "N/A",
                    Role = u.Roles
                        .Where(r => UserRole.GetCommunityRoles().Contains(r.Name))
                        .Select(r => r.Name)
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
                .Where(u => u.Roles.Any(r => r.Name == UserRole.ResidentOwner)
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
                    Status = u.Roles.Any(r => UserRole.GetCommunityRoles().Contains(r.Name))
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
                    Role = u.Roles
                        .Where(r => UserRole.GetCommunityRoles().Contains(r.Name))
                        .Select(r => r.Name)
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
                .AnyAsync(u => u.Roles.Any(r => r.Name == roleName));
        }

        public async Task AssignCommunityRoleAsync(Guid userId, string roleName)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstAsync(u => u.Id == userId);

            var role = await _context.Roles.FirstAsync(r => r.Name == roleName);
            user.Roles.Add(role);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCommunityRoleAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstAsync(u => u.Id == userId);

            var role = user.Roles
                .First(r => UserRole.GetCommunityRoles().Contains(r.Name));

            user.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
