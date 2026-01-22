using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Interfaces.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Infrastructure.Repositories;

public class StaffMemberRepository : IStaffMemberRepository
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly IPasswordHasher _passwordHasher;

    public StaffMemberRepository(
        AppDbContext context,
        IUserRepository userRepo,
        IRoleRepository roleRepo,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<StaffMemberDto>> GetAllAsync()
    {
        return await _context.StaffMembers
            .AsNoTracking()
            .OrderByDescending(s => s.JoinedOn)
            .Select(s => new StaffMemberDto
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
                HourlyRate = s.HourlyRate
            })
            .ToListAsync();
    }

    public async Task<List<StaffMemberDto>> GetByTypeAsync(string staffType)
    {
        return await _context.StaffMembers
            .AsNoTracking()
            .Where(s => s.StaffType == staffType)
            .OrderByDescending(s => s.JoinedOn)
            .Select(s => new StaffMemberDto
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
                HourlyRate = s.HourlyRate
            })
            .ToListAsync();
    }

    public async Task<StaffMemberDto?> GetByIdAsync(Guid staffId)
    {
        return await _context.StaffMembers
            .AsNoTracking()
            .Where(s => s.Id == staffId)
            .Select(s => new StaffMemberDto
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
                HourlyRate = s.HourlyRate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> PhoneExistsAsync(string phone)
    {
        return await _context.StaffMembers
            .AsNoTracking()
            .AnyAsync(s => s.Phone == phone);
    }

    public async Task CreateAsync(CreateStaffMemberDto dto, Guid createdBy)
    {
        User? user = null;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var role = await _roleRepo.GetByNameAsync(dto.StaffType)
                ?? throw new Exception("Invalid staff role");

            user = new User
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email!,
                PrimaryPhone = dto.Phone,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UserRoles = new List<UserRole>
                {
                    new UserRole { RoleId = role.Id }
                }
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
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
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.StaffMembers.Add(staff);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateStaffMemberDto dto, Guid updatedBy)
    {
        var staff = await _context.StaffMembers
            .Include(s => s.User)
            .FirstAsync(s => s.Id == dto.StaffId);

        staff.FullName = dto.FullName;
        staff.Phone = dto.Phone;
        staff.Email = dto.Email;
        staff.Address = dto.Address;
        staff.Specialization = dto.Specialization;
        staff.HourlyRate = dto.HourlyRate;
        staff.IsActive = dto.IsActive;
        staff.UpdatedBy = updatedBy;
        staff.UpdatedAt = DateTime.UtcNow;

        if (staff.User != null)
        {
            staff.User.FullName = dto.FullName;
            staff.User.PrimaryPhone = dto.Phone;
            staff.User.Email = dto.Email;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SetActiveStatusAsync(Guid staffId, bool isActive, Guid updatedBy)
    {
        var staff = await _context.StaffMembers.FirstAsync(s => s.Id == staffId);

        staff.IsActive = isActive;
        staff.UpdatedBy = updatedBy;
        staff.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}





















/*using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ApartmentManagementSystem.Application.Interfaces.Services;

namespace ApartmentManagementSystem.Infrastructure.Repositories
{
    public class StaffMemberRepository : IStaffMemberRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPasswordHasher _passwordHasher;

        public StaffMemberRepository(
            AppDbContext context,
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<StaffMemberDto>> GetAllAsync()
        {
            return await _context.StaffMembers
                .OrderByDescending(s => s.JoinedOn)
                .Select(s => MapToDto(s))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<StaffMemberDto>> GetByTypeAsync(string staffType)
        {
            return await _context.StaffMembers
                .Where(s => s.StaffType == staffType)
                .OrderByDescending(s => s.JoinedOn)
                .Select(s => MapToDto(s))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<StaffMemberDto?> GetByIdAsync(Guid staffId)
        {
            return await _context.StaffMembers
                .Where(s => s.Id == staffId)
                .Select(s => MapToDto(s))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.StaffMembers.AnyAsync(s => s.Phone == phone);
        }

        public async Task CreateAsync(CreateStaffMemberDto dto, Guid createdBy)
        {
            User? user = null;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var role = await _roleRepo.GetByNameAsync(dto.StaffType);

                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    Email = dto.Email!,
                    PrimaryPhone = dto.Phone,
                    PasswordHash = _passwordHasher.HashPassword(dto.Password),
                    CreatedAt = DateTime.UtcNow,
                    Roles = new List<Role> { role! }
                };

                await _userRepo.CreateAsync(user);
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
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.StaffMembers.Add(staff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateStaffMemberDto dto, Guid updatedBy)
        {
            var staff = await _context.StaffMembers
                .Include(s => s.User)
                .FirstAsync(s => s.Id == dto.StaffId);

            staff.FullName = dto.FullName;
            staff.Phone = dto.Phone;
            staff.Email = dto.Email;
            staff.Address = dto.Address;
            staff.Specialization = dto.Specialization;
            staff.HourlyRate = dto.HourlyRate;
            staff.IsActive = dto.IsActive;
            staff.UpdatedBy = updatedBy;
            staff.UpdatedAt = DateTime.UtcNow;

            if (staff.User != null)
            {
                staff.User.FullName = dto.FullName;
                staff.User.PrimaryPhone = dto.Phone;
                staff.User.Email = dto.Email;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(Guid staffId, bool isActive, Guid updatedBy)
        {
            var staff = await _context.StaffMembers.FirstAsync(s => s.Id == staffId);

            staff.IsActive = isActive;
            staff.UpdatedBy = updatedBy;
            staff.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private static StaffMemberDto MapToDto(StaffMember s)
        {
            return new StaffMemberDto
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
                HourlyRate = s.HourlyRate
            };
        }
    }
}
*/