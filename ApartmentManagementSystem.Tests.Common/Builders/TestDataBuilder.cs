using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Tests.Common.Builders
{
    // ApartmentManagementSystem.Tests.Common/Builders/TestDataBuilder.cs
    using ApartmentManagementSystem.Domain.Entities;
    using ApartmentManagementSystem.Domain.Enums;

 
        public class TestDataBuilder
        {
            public static User CreateTestUser(
                string fullName = "Test User",
                string phone = "1234567890",
                string? email = "test@example.com",
                bool isActive = true)
            {
                return new User
                {
                    Id = Guid.NewGuid(),
                    FullName = fullName,
                    PrimaryPhone = phone,
                    Email = email,
                    IsActive = isActive,
                    IsOtpVerified = true,
                    IsRegistrationCompleted = true,
                    Status = ResidentStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                    UserRoles = new List<UserRole>()
                };
            }

            public static Role CreateTestRole(string roleName = "TestRole")
            {
                return new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    Description = $"{roleName} Description"
                };
            }

            public static Apartment CreateTestApartment(
                string name = "Test Apartment",
                int totalFloors = 5,
                int flatsPerFloor = 4)
            {
                return new Apartment
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Address = "123 Test Street",
                    City = "Test City",
                    State = "Test State",
                    PinCode = "123456",
                    TotalFloors = totalFloors,
                    FlatsPerFloor = flatsPerFloor,
                    TotalFlats = totalFloors * flatsPerFloor,
                    Status = ApartmentStatus.Active,
                    IsActive = true,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    Floors = new List<Floor>(),
                    Flats = new List<Flat>(),
                    Managers = new List<ApartmentManager>(),
                    CommunityMembers = new List<CommunityMember>()
                };
            }

            public static Floor CreateTestFloor(
                Guid apartmentId,
                int floorNumber = 1)
            {
                return new Floor
                {
                    Id = Guid.NewGuid(),
                    FloorNumber = floorNumber,
                    Name = $"Floor {floorNumber}",
                    ApartmentId = apartmentId,
                    Flats = new List<Flat>()
                };
            }

            public static Flat CreateTestFlat(
                Guid apartmentId,
                Guid floorId,
                string flatNumber = "101")
            {
                return new Flat
                {
                    Id = Guid.NewGuid(),
                    FlatNumber = flatNumber,
                    Name = $"Flat {flatNumber}",
                    ApartmentId = apartmentId,
                    FloorId = floorId,
                    IsOccupied = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UserFlatMappings = new List<UserFlatMapping>()
                };
            }

            public static CommunityMember CreateTestCommunityMember(
                Guid userId,
                Guid apartmentId,
                string role = "President")
            {
                return new CommunityMember
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ApartmentId = apartmentId,
                    CommunityRole = role,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = Guid.NewGuid(),
                    IsActive = true
                };
            }

            public static StaffMember CreateTestStaffMember(
                string fullName = "Test Staff",
                string staffType = "Security")
            {
                return new StaffMember
                {
                    Id = Guid.NewGuid(),
                    FullName = fullName,
                    Phone = "9876543210",
                    Email = "staff@example.com",
                    Address = "Test Address",
                    StaffType = staffType,
                    IsActive = true,
                    JoinedOn = DateTime.UtcNow,
                    CreatedBy = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                };
            }
        }
    }
