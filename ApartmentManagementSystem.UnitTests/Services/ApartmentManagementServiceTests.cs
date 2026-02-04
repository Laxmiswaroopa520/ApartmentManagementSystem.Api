// ApartmentManagementSystem.UnitTests/Services/ApartmentManagementServiceTests.cs
using ApartmentManagementSystem.Application.DTOs.Apartment;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Entities;
using ApartmentManagementSystem.Domain.Enums;
using ApartmentManagementSystem.Tests.Common.Builders;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.UnitTests.Services
{
    public class ApartmentManagementServiceTests
    {
        private readonly Mock<IApartmentRepository> _mockApartmentRepo;
        private readonly Mock<IFloorRepository> _mockFloorRepo;
        private readonly Mock<IFlatRepository> _mockFlatRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly ApartmentManagementService _service;

        public ApartmentManagementServiceTests()
        {
            _mockApartmentRepo = new Mock<IApartmentRepository>();
            _mockFloorRepo = new Mock<IFloorRepository>();
            _mockFlatRepo = new Mock<IFlatRepository>();
            _mockUserRepo = new Mock<IUserRepository>();

            _service = new ApartmentManagementService(
                _mockApartmentRepo.Object,
                _mockFloorRepo.Object,
                _mockFlatRepo.Object,
                _mockUserRepo.Object
            );
        }

        [Fact]
        public async Task CreateApartmentAsync_WithValidData_CreatesApartmentWithFloorsAndFlats()
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var dto = new CreateApartmentDto
            {
                Name = "Sunrise Apartments",
                Address = "123 Main St",
                City = "Test City",
                State = "Test State",
                PinCode = "123456",
                TotalFloors = 3,
                FlatsPerFloor = 4
            };

            _mockApartmentRepo
                .Setup(r => r.AddAsync(It.IsAny<Apartment>()))
                .Returns(Task.CompletedTask);

            _mockFloorRepo
                .Setup(r => r.AddAsync(It.IsAny<Floor>()))
                .Returns(Task.CompletedTask);

            _mockFlatRepo
                .Setup(r => r.AddAsync(It.IsAny<Flat>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateApartmentAsync(dto, createdBy);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            result.TotalFloors.Should().Be(dto.TotalFloors);
            result.TotalFlats.Should().Be(dto.TotalFloors * dto.FlatsPerFloor);
            result.FloorsCreated.Should().HaveCount(dto.TotalFloors);

            foreach (var floor in result.FloorsCreated)
            {
                floor.FlatNumbers.Should().HaveCount(dto.FlatsPerFloor);
            }

            _mockApartmentRepo.Verify(r => r.AddAsync(It.IsAny<Apartment>()), Times.Once);
            _mockFloorRepo.Verify(r => r.AddAsync(It.IsAny<Floor>()), Times.Exactly(dto.TotalFloors));
            _mockFlatRepo.Verify(r => r.AddAsync(It.IsAny<Flat>()), Times.Exactly(dto.TotalFloors * dto.FlatsPerFloor));
        }

        [Fact]
        public async Task GetAllApartmentsAsync_ReturnsAllApartments()
        {
            // Arrange
            var apartments = new List<Apartment>
            {
                TestDataBuilder.CreateTestApartment("Building A", 5, 4),
                TestDataBuilder.CreateTestApartment("Building B", 3, 6)
            };

            // Add flats for occupancy calculation
            apartments[0].Flats.Add(TestDataBuilder.CreateTestFlat(apartments[0].Id, Guid.NewGuid(), "101"));
            apartments[0].Flats.Add(TestDataBuilder.CreateTestFlat(apartments[0].Id, Guid.NewGuid(), "102"));
            apartments[0].Flats[0].IsOccupied = true;

            _mockApartmentRepo
                .Setup(r => r.GetAllWithDetailsAsync())
                .ReturnsAsync(apartments);

            // Act
            var result = await _service.GetAllApartmentsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Building A");
            result.First().OccupiedFlats.Should().Be(1);
        }

        [Fact]
        public async Task GetApartmentDetailAsync_WithValidId_ReturnsApartmentDetails()
        {
            // Arrange
            var apartmentId = Guid.NewGuid();
            var apartment = TestDataBuilder.CreateTestApartment("Test Apartment", 5, 4);
            apartment.Id = apartmentId;

            // Add manager
            var manager = new ApartmentManager
            {
                Id = Guid.NewGuid(),
                ApartmentId = apartmentId,
                UserId = Guid.NewGuid(),
                IsActive = true,
                AssignedAt = DateTime.UtcNow,
                User = TestDataBuilder.CreateTestUser("Manager User")
            };
            apartment.Managers.Add(manager);

            // Add community members
            var president = TestDataBuilder.CreateTestCommunityMember(
                Guid.NewGuid(), apartmentId, "President");
            president.User = TestDataBuilder.CreateTestUser("President User");
            apartment.CommunityMembers.Add(president);

            // Add flats
            for (int i = 0; i < 10; i++)
            {
                var flat = TestDataBuilder.CreateTestFlat(apartmentId, Guid.NewGuid(), $"10{i}");
                flat.IsOccupied = i < 6; // 6 occupied
                apartment.Flats.Add(flat);
            }

            _mockApartmentRepo
                .Setup(r => r.GetByIdWithFullDetailsAsync(apartmentId))
                .ReturnsAsync(apartment);

            // Act
            var result = await _service.GetApartmentDetailAsync(apartmentId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(apartmentId);
            result.Name.Should().Be("Test Apartment");
            result.TotalFlats.Should().Be(20); // 5 floors * 4 flats
            result.OccupiedFlats.Should().Be(6);
            result.VacantFlats.Should().Be(4);
            result.Manager.Should().NotBeNull();
            result.Manager!.FullName.Should().Be("Manager User");
            result.President.Should().NotBeNull();
        }

        [Fact]
        public async Task GetApartmentDetailAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var apartmentId = Guid.NewGuid();

            _mockApartmentRepo
                .Setup(r => r.GetByIdWithFullDetailsAsync(apartmentId))
                .ReturnsAsync((Apartment?)null);

            // Act
            var result = await _service.GetApartmentDetailAsync(apartmentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetApartmentDiagramAsync_WithValidId_ReturnsDiagram()
        {
            // Arrange
            var apartmentId = Guid.NewGuid();
            var apartment = TestDataBuilder.CreateTestApartment("Test Apartment", 3, 4);
            apartment.Id = apartmentId;

            // Create floors with flats
            for (int f = 1; f <= 3; f++)
            {
                var floor = TestDataBuilder.CreateTestFloor(apartmentId, f);
                floor.Id = Guid.NewGuid();

                for (int flat = 1; flat <= 4; flat++)
                {
                    var flatEntity = TestDataBuilder.CreateTestFlat(
                        apartmentId, floor.Id, $"{f}0{flat}");
                    flatEntity.IsOccupied = flat <= 2; // First 2 flats occupied
                    floor.Flats.Add(flatEntity);
                }

                apartment.Floors.Add(floor);
            }

            _mockApartmentRepo
                .Setup(r => r.GetByIdWithFloorsAndFlatsAsync(apartmentId))
                .ReturnsAsync(apartment);

            // Act
            var result = await _service.GetApartmentDiagramAsync(apartmentId);

            // Assert
            result.Should().NotBeNull();
            result.ApartmentId.Should().Be(apartmentId);
            result.Name.Should().Be("Test Apartment");
            result.TotalFloors.Should().Be(3);
            result.Floors.Should().HaveCount(3);

            foreach (var floor in result.Floors)
            {
                floor.Flats.Should().HaveCount(4);
            }
        }

        [Fact]
        public async Task GetApartmentDiagramAsync_WithNoFloors_ThrowsException()
        {
            // Arrange
            var apartmentId = Guid.NewGuid();
            var apartment = TestDataBuilder.CreateTestApartment("Test Apartment", 3, 4);
            apartment.Id = apartmentId;
            apartment.Floors.Clear(); // No floors

            _mockApartmentRepo
                .Setup(r => r.GetByIdWithFloorsAndFlatsAsync(apartmentId))
                .ReturnsAsync(apartment);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.GetApartmentDiagramAsync(apartmentId)
            );
        }

        [Fact]
        public async Task AssignManagerAsync_WithValidData_AssignsManager()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();
            var assignedBy = Guid.NewGuid();

            var dto = new AssignManagerDto
            {
                UserId = userId,
                ApartmentId = apartmentId
            };

            var role = TestDataBuilder.CreateTestRole("Manager");
            var user = TestDataBuilder.CreateTestUser("Manager User");
            user.Id = userId;
            user.UserRoles.Add(new UserRole { UserId = userId, RoleId = role.Id, Role = role });

            _mockUserRepo
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _mockApartmentRepo
                .Setup(r => r.GetActiveManagerAsync(apartmentId))
                .ReturnsAsync((ApartmentManager?)null);

            _mockApartmentRepo
                .Setup(r => r.AddManagerAsync(It.IsAny<ApartmentManager>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AssignManagerAsync(dto, assignedBy);

            // Assert
            result.Should().BeTrue();
            _mockApartmentRepo.Verify(r => r.AddManagerAsync(It.IsAny<ApartmentManager>()), Times.Once);
        }

        [Fact]
        public async Task AssignManagerAsync_WithNonExistentUser_ThrowsException()
        {
            // Arrange
            var dto = new AssignManagerDto
            {
                UserId = Guid.NewGuid(),
                ApartmentId = Guid.NewGuid()
            };

            _mockUserRepo
                .Setup(r => r.GetByIdAsync(dto.UserId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _service.AssignManagerAsync(dto, Guid.NewGuid())
            );
        }

        [Fact]
        public async Task AssignManagerAsync_WithUserWithoutManagerRole_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new AssignManagerDto
            {
                UserId = userId,
                ApartmentId = Guid.NewGuid()
            };

            var user = TestDataBuilder.CreateTestUser();
            user.Id = userId;
            // No Manager role

            _mockUserRepo
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.AssignManagerAsync(dto, Guid.NewGuid())
            );

            exception.Message.Should().Contain("Manager role");
        }

        [Fact]
        public async Task AssignManagerAsync_ReplacesExistingManager()
        {
            // Arrange
            var newUserId = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();
            var assignedBy = Guid.NewGuid();

            var dto = new AssignManagerDto
            {
                UserId = newUserId,
                ApartmentId = apartmentId
            };

            var role = TestDataBuilder.CreateTestRole("Manager");
            var newUser = TestDataBuilder.CreateTestUser("New Manager");
            newUser.Id = newUserId;
            newUser.UserRoles.Add(new UserRole { UserId = newUserId, RoleId = role.Id, Role = role });

            var existingManager = new ApartmentManager
            {
                Id = Guid.NewGuid(),
                ApartmentId = apartmentId,
                UserId = Guid.NewGuid(),
                IsActive = true
            };

            _mockUserRepo
                .Setup(r => r.GetByIdAsync(newUserId))
                .ReturnsAsync(newUser);

            _mockApartmentRepo
                .Setup(r => r.GetActiveManagerAsync(apartmentId))
                .ReturnsAsync(existingManager);

            _mockApartmentRepo
                .Setup(r => r.UpdateManagerAsync(It.IsAny<ApartmentManager>()))
                .Returns(Task.CompletedTask);

            _mockApartmentRepo
                .Setup(r => r.AddManagerAsync(It.IsAny<ApartmentManager>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AssignManagerAsync(dto, assignedBy);

            // Assert
            result.Should().BeTrue();
            _mockApartmentRepo.Verify(r => r.UpdateManagerAsync(
                It.Is<ApartmentManager>(m => m.IsActive == false)
            ), Times.Once);
            _mockApartmentRepo.Verify(r => r.AddManagerAsync(It.IsAny<ApartmentManager>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateApartmentAsync_DeactivatesApartment()
        {
            // Arrange
            var apartmentId = Guid.NewGuid();
            var deactivatedBy = Guid.NewGuid();
            var apartment = TestDataBuilder.CreateTestApartment();
            apartment.Id = apartmentId;
            apartment.IsActive = true;

            _mockApartmentRepo
                .Setup(r => r.GetByIdAsync(apartmentId))
                .ReturnsAsync(apartment);

            _mockApartmentRepo
                .Setup(r => r.UpdateAsync(It.IsAny<Apartment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeactivateApartmentAsync(apartmentId, deactivatedBy);

            // Assert
            result.Should().BeTrue();
            _mockApartmentRepo.Verify(r => r.UpdateAsync(
                It.Is<Apartment>(a => a.IsActive == false)
            ), Times.Once);
        }

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(5, 4, 20)]
        [InlineData(10, 6, 60)]
        public async Task CreateApartmentAsync_WithDifferentConfigurations_CreatesCorrectNumberOfFlats(
            int floors, int flatsPerFloor, int expectedTotal)
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var dto = new CreateApartmentDto
            {
                Name = "Test Building",
                Address = "Test Address",
                TotalFloors = floors,
                FlatsPerFloor = flatsPerFloor
            };

            _mockApartmentRepo.Setup(r => r.AddAsync(It.IsAny<Apartment>())).Returns(Task.CompletedTask);
            _mockFloorRepo.Setup(r => r.AddAsync(It.IsAny<Floor>())).Returns(Task.CompletedTask);
            _mockFlatRepo.Setup(r => r.AddAsync(It.IsAny<Flat>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateApartmentAsync(dto, createdBy);

            // Assert
            result.TotalFlats.Should().Be(expectedTotal);
            _mockFlatRepo.Verify(r => r.AddAsync(It.IsAny<Flat>()), Times.Exactly(expectedTotal));
        }
    }
}