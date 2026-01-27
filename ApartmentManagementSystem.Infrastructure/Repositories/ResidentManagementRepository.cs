using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ResidentManagementRepository : IResidentManagementRepository
    {
        private readonly AppDbContext DBContext;

        public ResidentManagementRepository(AppDbContext context)
        {
            DBContext = context;
        }

        public async Task<List<ResidentListDto>> GetAllResidentsAsync()
        {
            return await DBContext.Users
    .Where(u => u.UserRoles.Any(ur =>
        ur.Role.Name == RoleNames.ResidentOwner ||
        ur.Role.Name == RoleNames.Tenant))
    .Select(u => new ResidentListDto
    {
        UserId = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Phone = u.PrimaryPhone,

        ResidentType = u.UserRoles.Any(ur => ur.Role.Name == RoleNames.ResidentOwner)
            ? "Owner"
            : "Tenant",

        FlatNumber = u.UserFlatMappings
            .Select(f => f.Flat.FlatNumber)
            .FirstOrDefault(),

        Status = !u.IsActive
            ? "Inactive"
            : !u.UserFlatMappings.Any()
                ? "Pending Assignment"
                : "Active",

        RegisteredOn = u.CreatedAt
    })
    .OrderByDescending(r => r.RegisteredOn)
    .AsNoTracking()
    .ToListAsync();

            //return await _context.Users
            //    .Where(u => u.UserRoles.Any(ur =>
            //        ur.Role.Name == RoleNames.ResidentOwner ||
            //        ur.Role.Name == RoleNames.Tenant))
            //    .Select(u => new ResidentListDto
            //    {
            //        UserId = u.Id,
            //        FullName = u.FullName,
            //        Email = u.Email,
            //        Phone = u.PrimaryPhone,
            //        ResidentType = u.UserRoles.Any(ur => ur.Role.Name == RoleNames.ResidentOwner)
            //            ? "Owner"
            //            : "Tenant",
            //        FlatNumber = u.UserFlatMappings
            //            .Select(f => f.Flat.FlatNumber)
            //            .FirstOrDefault(),
            //        Status = u.UserFlatMappings.Any()
            //            ? "Active"
            //            : "Pending Assignment",
            //        RegisteredOn = u.CreatedAt
            //    })
            //    .OrderByDescending(r => r.RegisteredOn)
            //    .AsNoTracking()
            //    .ToListAsync();
        }

        public async Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType)
        {
            var roleName = residentType.ToLower() == "owner"
                ? RoleNames.ResidentOwner
                : RoleNames.Tenant;

            return await DBContext.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    ResidentType = residentType,
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault(),
                    Status = u.UserFlatMappings.Any()
                        ? "Active"
                        : "Pending Assignment",
                    RegisteredOn = u.CreatedAt
                })
                .OrderByDescending(r => r.RegisteredOn)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId)
        {
            return await DBContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new ResidentDetailDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PrimaryPhone = u.PrimaryPhone,
                    SecondaryPhone = u.SecondaryPhone,
                    ResidentType = u.UserRoles.Any(ur => ur.Role.Name == RoleNames.ResidentOwner)
                        ? "Owner"
                        : "Tenant",
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault(),
                    ApartmentName = u.UserFlatMappings
                        .Select(f => f.Flat.Apartment.Name)
                        .FirstOrDefault(),
                    RegisteredOn = u.CreatedAt,
                    Status = u.UserFlatMappings.Any()
                        ? "Active"
                        : "Pending Assignment",
                    Roles = u.UserRoles
                        .Select(ur => ur.Role.Name)
                        .ToList(),
                    TotalComplaints = 0,
                    OutstandingBills = 0
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task SetResidentActiveStatusAsync(
            Guid userId, bool isActive, Guid updatedBy)
        {
            var user = await DBContext.Users.FirstAsync(u => u.Id == userId);

            user.IsActive = isActive;
            user.UpdatedBy = updatedBy;   
            user.UpdatedAt = DateTime.UtcNow;

            await DBContext.SaveChangesAsync();
        }
    }
}




























/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.Resident_Management;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class ResidentManagementRepository : IResidentManagementRepository
    {
        private readonly AppDbContext _context;

        public ResidentManagementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResidentListDto>> GetAllResidentsAsync()
        {
            return await _context.Users
               /* .Where(u => u.Roles.Any(r =>
                    r.Name == RoleNames.ResidentOwner || r.Name == RoleNames.Tenant))-------------
               .Where(u => u.UserRoles.Any(ur =>
    ur.Role.Name == RoleNames.ResidentOwner ||
    ur.Role.Name == RoleNames.Tenant))

                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    ResidentType = u.Roles.Any(r => r.Name == RoleNames.ResidentOwner)
                        ? "Owner"
                        : "Tenant",
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault(),
                    Status = u.UserFlatMappings.Any()
                        ? "Active"
                        : "Pending Assignment",
                    RegisteredOn = u.CreatedAt
                })
                .OrderByDescending(r => r.RegisteredOn)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ResidentListDto>> GetResidentsByTypeAsync(string residentType)
        {
            var roleName = residentType.ToLower() == "owner"
                ? RoleNames.ResidentOwner
                : RoleNames.Tenant;

            return await _context.Users
                .Where(u => u.Roles.Any(r => r.Name == roleName))
                .Select(u => new ResidentListDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PrimaryPhone,
                    ResidentType = residentType,
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault(),
                    Status = u.UserFlatMappings.Any()
                        ? "Active"
                        : "Pending Assignment",
                    RegisteredOn = u.CreatedAt
                })
                .OrderByDescending(r => r.RegisteredOn)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ResidentDetailDto?> GetResidentDetailAsync(Guid userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new ResidentDetailDto
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PrimaryPhone = u.PrimaryPhone,
                    SecondaryPhone = u.SecondaryPhone,
                    ResidentType = u.Roles.Any(r => r.Name == RoleNames.ResidentOwner)
                        ? "Owner"
                        : "Tenant",
                    FlatNumber = u.UserFlatMappings
                        .Select(f => f.Flat.FlatNumber)
                        .FirstOrDefault(),
                    ApartmentName = u.UserFlatMappings
                        .Select(f => f.Flat.Apartment.Name)
                        .FirstOrDefault(),
                    RegisteredOn = u.CreatedAt,
                    Status = u.UserFlatMappings.Any()
                        ? "Active"
                        : "Pending Assignment",
                    Roles = u.Roles.Select(r => r.Name).ToList(),
                    TotalComplaints = 0,
                    OutstandingBills = 0
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task SetResidentActiveStatusAsync(
            Guid userId, bool isActive, Guid updatedBy)
        {
            var user = await _context.Users.FirstAsync(u => u.Id == userId);

            user.IsActive = isActive;
            user.UpdatedBy = updatedBy;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
*/