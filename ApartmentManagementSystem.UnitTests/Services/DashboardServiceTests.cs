// ApartmentManagementSystem.UnitTests/Services/DashboardServiceTests.cs
using ApartmentManagementSystem.Application.DTOs.Dashboard;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Tests.Common.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.UnitTests.Services
{
    public class DashboardServiceTests
    {
        private readonly Mock<IUserRepository> MockUserRepository;
        private readonly Mock<IFlatRepository> MockFlatRepository;
        private readonly Mock<IApartmentRepository> MockApartmentRepository;
        private readonly Mock<IUserFlatMappingRepository> MockUserFlatMappingRepository;
        private readonly DashboardService Service;

        public DashboardServiceTests()
        {
            MockUserRepository = new Mock<IUserRepository>();
            MockFlatRepository = new Mock<IFlatRepository>();
            MockApartmentRepository = new Mock<IApartmentRepository>();
            MockUserFlatMappingRepository = new Mock<IUserFlatMappingRepository>();

            Service = new DashboardService(
                MockUserRepository.Object,
                MockFlatRepository.Object,
                MockApartmentRepository.Object,
                MockUserFlatMappingRepository.Object
            );
        }

        [Fact]
        public async Task GetAdminDashboardAsync_WithValidUserId_ReturnsDashboard()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = TestDataBuilder.CreateTestRole("SuperAdmin");
            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;
            user.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                Role = role
            });

            MockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            MockFlatRepository
                .Setup(r => r.GetTotalCountAsync())
                .ReturnsAsync(100);

            MockFlatRepository
                .Setup(r => r.GetOccupiedCountAsync())
                .ReturnsAsync(75);

            // Act
            var result = await Service.GetAdminDashboardAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(user.FullName);
            result.Role.Should().Be("SuperAdmin");
            result.Stats.Should().NotBeNull();
            result.Stats.TotalFlats.Should().Be(100);
            result.Stats.OccupiedFlats.Should().Be(75);
            result.Stats.VacantFlats.Should().Be(25);
        }

        [Fact]
        public async Task GetAdminDashboardAsync_WithNonExistentUser_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            MockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => Service.GetAdminDashboardAsync(userId)
            );
        }

        [Fact]
        public async Task GetOwnerDashboardAsync_WithValidUserId_ReturnsDashboard()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;

            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);
            var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, "101");
            flat.OwnerUserId = userId;
            flat.OwnerUser = user;
            flat.Apartment = apartment;

            MockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            MockFlatRepository
                .Setup(r => r.GetFlatsWithMappingsByOwnerIdAsync(userId))
                .ReturnsAsync(new List<Flat> { flat });

            // Act
            var result = await Service.GetOwnerDashboardAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(user.FullName);
            result.UserId.Should().Be(userId);
            result.MyFlats.Should().HaveCount(1);
            result.MyFlats.First().FlatNumber.Should().Be("101");
        }

        [Fact]
        public async Task GetTenantDashboardAsync_WithValidUserId_ReturnsDashboard()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;

            var apartment = TestDataBuilder.CreateTestApartment();
            var floor = TestDataBuilder.CreateTestFloor(apartment.Id);
            var flat = TestDataBuilder.CreateTestFlat(apartment.Id, floor.Id, "102");
            flat.Apartment = apartment;

            var mapping = new UserFlatMapping
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FlatId = flat.Id,
                IsActive = true,
                Flat = flat,
                User = user
            };

            MockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            MockUserFlatMappingRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<UserFlatMapping> { mapping });

            // Act
            var result = await Service.GetTenantDashboardAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(user.FullName);
            result.UserId.Should().Be(userId);
            result.MyFlat.Should().NotBeNull();
            result.MyFlat!.FlatNumber.Should().Be("102");
        }

        [Fact]
        public async Task GetDashboardStatsAsync_ReturnsCorrectStats()
        {
            // Arrange
            MockFlatRepository
                .Setup(r => r.GetTotalCountAsync())
                .ReturnsAsync(150);

            MockFlatRepository
                .Setup(r => r.GetOccupiedCountAsync())
                .ReturnsAsync(120);

            // Act
            var result = await Service.GetDashboardStatsAsync();

            // Assert
            result.Should().NotBeNull();
            result.TotalFlats.Should().Be(150);
            result.OccupiedFlats.Should().Be(120);
            result.VacantFlats.Should().Be(30);
        }

        [Fact]
        public async Task GetOwnerDashboardAsync_WithNoFlats_ReturnsEmptyFlatsList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;

            MockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            MockFlatRepository
                .Setup(r => r.GetFlatsWithMappingsByOwnerIdAsync(userId))
                .ReturnsAsync(new List<Flat>());

            // Act
            var result = await Service.GetOwnerDashboardAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.MyFlats.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTenantDashboardAsync_WithNoMapping_ReturnsNullFlat()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;

            MockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            MockUserFlatMappingRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<UserFlatMapping>());

            // Act
            var result = await Service.GetTenantDashboardAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.MyFlat.Should().BeNull();
        }

        [Theory]
        [InlineData("SuperAdmin")]
        [InlineData("Manager")]
        [InlineData("President")]
        public async Task GetAdminDashboardAsync_WithDifferentRoles_ReturnsCorrectRole(string roleName)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = TestDataBuilder.CreateTestRole(roleName);
            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;
            user.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                Role = role
            });

            MockUserRepository
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            MockFlatRepository
                .Setup(r => r.GetTotalCountAsync())
                .ReturnsAsync(50);

            MockFlatRepository
                .Setup(r => r.GetOccupiedCountAsync())
                .ReturnsAsync(30);

            // Act
            var result = await Service.GetAdminDashboardAsync(userId);

            // Assert
            result.Role.Should().Be(roleName);
        }
    }
}