using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class StaffMemberRepository : IStaffMemberRepository
{
    private readonly AppDbContext DBContext;
    private readonly IUserRepository UserRepo;
    private readonly IRoleRepository RoleRepo;
    private readonly IPasswordHasher PasswordHasher;

    public StaffMemberRepository(
        AppDbContext context,
        IUserRepository userRepo,
        IRoleRepository roleRepo,
        IPasswordHasher passwordHasher)
    {
        DBContext = context;
        UserRepo = userRepo;
        RoleRepo = roleRepo;
        PasswordHasher = passwordHasher;
    }

    private static StaffMemberDto MapToStaffDto(StaffMember s) => new()
    {
        StaffId = s.Id,
        FullName = s.FullName,
        Phone = s.Phone,
        Email = s.Email,
        Address = s.Address,
        StaffType = s.StaffType,
        JoinedOn = s.JoinedOn,
        IsActive = s.IsActive,
        Specialization = s.Specialization,
        HourlyRate = s.HourlyRate,
        ApartmentId = s.ApartmentId,
        ApartmentName = s.Apartment?.Name
    };

    private IQueryable<StaffMember> BaseQuery() =>
        DBContext.StaffMembers
            .AsNoTracking()
            .Include(s => s.Apartment)
            .OrderByDescending(s => s.JoinedOn);

    public async Task<List<StaffMemberDto>> GetAllAsync() =>
        (await BaseQuery().ToListAsync()).Select(MapToStaffDto).ToList();

    public async Task<List<StaffMemberDto>> GetByTypeAsync(string staffType) =>
        (await BaseQuery()
            .Where(s => s.StaffType == staffType)
            .ToListAsync())
        .Select(MapToStaffDto).ToList();

    public async Task<StaffMemberDto?> GetByIdAsync(Guid staffId)
    {
        var s = await DBContext.StaffMembers
            .AsNoTracking()
            .Include(x => x.Apartment)
            .FirstOrDefaultAsync(x => x.Id == staffId);
        return s is null ? null : MapToStaffDto(s);
    }

    public async Task<bool> PhoneExistsAsync(string phone) =>
        await DBContext.StaffMembers.AsNoTracking().AnyAsync(s => s.Phone == phone);

    public async Task CreateAsync(CreateStaffMemberDto dto, Guid createdBy)
    {
        User? user = null;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var role = await RoleRepo.GetByNameAsync(dto.StaffType)
                ?? throw new Exception("Invalid staff role");

            user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email!,
                PrimaryPhone = dto.Phone,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UserRoles = new List<UserRole> { new() { RoleId = role.Id } }
            };

            await UserRepo.AddAsync(user);
            await UserRepo.SaveChangesAsync();
        }

        var staff = new StaffMember
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            StaffType = dto.StaffType,
            Specialization = dto.Specialization,
            HourlyRate = dto.HourlyRate,
            IsActive = true,
            JoinedOn = DateTime.UtcNow,
            UserId = user?.Id,
            ApartmentId = dto.ApartmentId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        DBContext.StaffMembers.Add(staff);
        await DBContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateStaffMemberDto dto, Guid updatedBy)
    {
        var staff = await DBContext.StaffMembers
            .Include(s => s.User)
            .FirstAsync(s => s.Id == dto.StaffId);

        staff.FullName = dto.FullName;
        staff.Phone = dto.Phone;
        staff.Email = dto.Email;
        staff.Address = dto.Address;
        staff.Specialization = dto.Specialization;
        staff.HourlyRate = dto.HourlyRate;
        staff.IsActive = dto.IsActive;
        staff.ApartmentId = dto.ApartmentId;
        staff.UpdatedBy = updatedBy;
        staff.UpdatedAt = DateTime.UtcNow;

        if (staff.User != null)
        {
            staff.User.FullName = dto.FullName;
            staff.User.PrimaryPhone = dto.Phone;
            staff.User.Email = dto.Email;
        }

        await DBContext.SaveChangesAsync();
    }

    public async Task SetActiveStatusAsync(Guid staffId, bool isActive, Guid updatedBy)
    {
        var staff = await DBContext.StaffMembers.FirstAsync(s => s.Id == staffId);
        staff.IsActive = isActive;
        staff.UpdatedBy = updatedBy;
        staff.UpdatedAt = DateTime.UtcNow;
        await DBContext.SaveChangesAsync();
    }
}



























































/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class StaffMemberRepository : IStaffMemberRepository
{
    private readonly AppDbContext DBContext;
    private readonly IUserRepository UserRepo;
    private readonly IRoleRepository RoleRepo;
    private readonly IPasswordHasher PasswordHasher;

    public StaffMemberRepository(
        AppDbContext context,
        IUserRepository userRepo,
        IRoleRepository roleRepo,
        IPasswordHasher passwordHasher)
    {
        DBContext = context;
        UserRepo = userRepo;
        RoleRepo = roleRepo;
        PasswordHasher = passwordHasher;
    }

    private static StaffMemberDto MapToDto(StaffMember s) => new()
    {
        StaffId = s.Id,
        FullName = s.FullName,
        Phone = s.Phone,
        Email = s.Email,
        Address = s.Address,
        StaffType = s.StaffType,
        JoinedOn = s.JoinedOn,
        IsActive = s.IsActive,
        Specialization = s.Specialization,
        HourlyRate = s.HourlyRate,
        ApartmentId = s.ApartmentId,
        ApartmentName = s.Apartment?.Name
    };

    private IQueryable<StaffMember> BaseQuery() =>
        DBContext.StaffMembers
            .AsNoTracking()
            .Include(s => s.Apartment)
            .OrderByDescending(s => s.JoinedOn);

    public async Task<List<StaffMemberDto>> GetAllAsync() =>
        (await BaseQuery().ToListAsync()).Select(MapToDto).ToList();

    public async Task<List<StaffMemberDto>> GetByTypeAsync(string staffType) =>
        (await BaseQuery()
            .Where(s => s.StaffType == staffType)
            .ToListAsync())
        .Select(MapToDto).ToList();

    public async Task<StaffMemberDto?> GetByIdAsync(Guid staffId)
    {
        var s = await DBContext.StaffMembers
            .AsNoTracking()
            .Include(x => x.Apartment)
            .FirstOrDefaultAsync(x => x.Id == staffId);
        return s is null ? null : MapToDto(s);
    }

    public async Task<bool> PhoneExistsAsync(string phone) =>
        await DBContext.StaffMembers.AsNoTracking().AnyAsync(s => s.Phone == phone);

    public async Task CreateAsync(CreateStaffMemberDto dto, Guid createdBy)
    {
        User? user = null;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var role = await RoleRepo.GetByNameAsync(dto.StaffType)
                ?? throw new Exception("Invalid staff role");

            user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email!,
                PrimaryPhone = dto.Phone,
                PasswordHash = PasswordHasher.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UserRoles = new List<UserRole> { new() { RoleId = role.Id } }
            };

            await UserRepo.AddAsync(user);
            await UserRepo.SaveChangesAsync();
        }

        var staff = new StaffMember
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            StaffType = dto.StaffType,
            Specialization = dto.Specialization,
            HourlyRate = dto.HourlyRate,
            IsActive = true,
            JoinedOn = DateTime.UtcNow,
            UserId = user?.Id,
            ApartmentId = dto.ApartmentId,     
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        DBContext.StaffMembers.Add(staff);
        await DBContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateStaffMemberDto dto, Guid updatedBy)
    {
        var staff = await DBContext.StaffMembers
            .Include(s => s.User)
            .FirstAsync(s => s.Id == dto.StaffId);

        staff.FullName = dto.FullName;
        staff.Phone = dto.Phone;
        staff.Email = dto.Email;
        staff.Address = dto.Address;
        staff.Specialization = dto.Specialization;
        staff.HourlyRate = dto.HourlyRate;
        staff.IsActive = dto.IsActive;
        staff.ApartmentId = dto.ApartmentId;  
        staff.UpdatedBy = updatedBy;
        staff.UpdatedAt = DateTime.UtcNow;

        if (staff.User != null)
        {
            staff.User.FullName = dto.FullName;
            staff.User.PrimaryPhone = dto.Phone;
            staff.User.Email = dto.Email;
        }

        await DBContext.SaveChangesAsync();
    }

    public async Task SetActiveStatusAsync(Guid staffId, bool isActive, Guid updatedBy)
    {
        var staff = await DBContext.StaffMembers.FirstAsync(s => s.Id == staffId);
        staff.IsActive = isActive;
        staff.UpdatedBy = updatedBy;
        staff.UpdatedAt = DateTime.UtcNow;
        await DBContext.SaveChangesAsync();
    }
}
*/




































