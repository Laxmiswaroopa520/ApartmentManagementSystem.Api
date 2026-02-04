// ApartmentManagementSystem.UnitTests/Services/StaffMemberServiceTests.cs
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using ApartmentManagementSystem.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.UnitTests.Services
{
    public class StaffMemberServiceTests
    {
        private readonly Mock<IStaffMemberRepository> MockRepository;
        private readonly StaffMemberService Service;

        public StaffMemberServiceTests()
        {
            MockRepository = new Mock<IStaffMemberRepository>();
            Service = new StaffMemberService(MockRepository.Object);
        }

        [Fact]
        public async Task GetAllStaffMembersAsync_ReturnsAllStaff()
        {
            // Arrange
            var staffList = new List<StaffMemberDto>
            {
                new StaffMemberDto
                {
                    StaffId = Guid.NewGuid(),
                    FullName = "John Security",
                    StaffType = "Security",
                    IsActive = true
                },
                new StaffMemberDto
                {
                    StaffId = Guid.NewGuid(),
                    FullName = "Jane Plumber",
                    StaffType = "Plumber",
                    IsActive = true
                }
            };

            MockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(staffList);

            // Act
            var result = await Service.GetAllStaffMembersAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(staffList);
        }

        [Fact]
        public async Task GetStaffMembersByTypeAsync_ReturnsFilteredStaff()
        {
            // Arrange
            var staffType = "Security";
            var staffList = new List<StaffMemberDto>
            {
                new StaffMemberDto
                {
                    StaffId = Guid.NewGuid(),
                    FullName = "John Security",
                    StaffType = staffType,
                    IsActive = true
                }
            };

            MockRepository
                .Setup(r => r.GetByTypeAsync(staffType))
                .ReturnsAsync(staffList);

            // Act
            var result = await Service.GetStaffMembersByTypeAsync(staffType);

            // Assert
            result.Should().HaveCount(1);
            result.First().StaffType.Should().Be(staffType);
        }

        [Fact]
        public async Task GetStaffMemberByIdAsync_WithValidId_ReturnsStaffMember()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var staffMember = new StaffMemberDto
            {
                StaffId = staffId,
                FullName = "Test Staff",
                StaffType = "Electrician",
                IsActive = true
            };

            MockRepository
                .Setup(r => r.GetByIdAsync(staffId))
                .ReturnsAsync(staffMember);

            // Act
            var result = await Service.GetStaffMemberByIdAsync(staffId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(staffMember);
        }

        [Fact]
        public async Task CreateStaffMemberAsync_WithValidData_CreatesStaff()
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var createDto = new CreateStaffMemberDto
            {
                FullName = "New Staff",
                Phone = "1234567890",
                Email = "staff@example.com",
                StaffType = "Security",
                Address = "Test Address"
            };

            MockRepository
                .Setup(r => r.PhoneExistsAsync(createDto.Phone))
                .ReturnsAsync(false);

            MockRepository
                .Setup(r => r.CreateAsync(createDto, createdBy))
                .Returns(Task.CompletedTask);

            var createdStaff = new StaffMemberDto
            {
                StaffId = Guid.NewGuid(),
                FullName = createDto.FullName,
                Phone = createDto.Phone,
                StaffType = createDto.StaffType,
                IsActive = true
            };

            MockRepository
                .Setup(r => r.GetByTypeAsync(createDto.StaffType))
                .ReturnsAsync(new List<StaffMemberDto> { createdStaff });

            // Act
            var result = await Service.CreateStaffMemberAsync(createDto, createdBy);

            // Assert
            result.Should().NotBeNull();
            result.Phone.Should().Be(createDto.Phone);
            MockRepository.Verify(r => r.CreateAsync(createDto, createdBy), Times.Once);
        }

        [Fact]
        public async Task CreateStaffMemberAsync_WithInvalidStaffType_ThrowsException()
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var createDto = new CreateStaffMemberDto
            {
                FullName = "New Staff",
                Phone = "1234567890",
                StaffType = "InvalidType"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Service.CreateStaffMemberAsync(createDto, createdBy)
            );

            exception.Message.Should().Contain("Invalid staff type");
        }

        [Fact]
        public async Task CreateStaffMemberAsync_WithExistingPhone_ThrowsException()
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var createDto = new CreateStaffMemberDto
            {
                FullName = "New Staff",
                Phone = "1234567890",
                StaffType = "Security"
            };

            MockRepository
                .Setup(r => r.PhoneExistsAsync(createDto.Phone))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Service.CreateStaffMemberAsync(createDto, createdBy)
            );

            exception.Message.Should().Contain("already exists");
        }

        [Fact]
        public async Task UpdateStaffMemberAsync_WithValidData_UpdatesStaff()
        {
            // Arrange
            var updatedBy = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var updateDto = new UpdateStaffMemberDto
            {
                StaffId = staffId,
                FullName = "Updated Name",
                Phone = "9876543210",
                IsActive = true
            };

            MockRepository
                .Setup(r => r.UpdateAsync(updateDto, updatedBy))
                .Returns(Task.CompletedTask);

            var updatedStaff = new StaffMemberDto
            {
                StaffId = staffId,
                FullName = updateDto.FullName,
                Phone = updateDto.Phone,
                IsActive = true
            };

            MockRepository
                .Setup(r => r.GetByIdAsync(staffId))
                .ReturnsAsync(updatedStaff);

            // Act
            var result = await Service.UpdateStaffMemberAsync(updateDto, updatedBy);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(updateDto.FullName);
            MockRepository.Verify(r => r.UpdateAsync(updateDto, updatedBy), Times.Once);
        }

        [Fact]
        public async Task DeactivateStaffMemberAsync_DeactivatesStaff()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var deactivatedBy = Guid.NewGuid();

            MockRepository
                .Setup(r => r.SetActiveStatusAsync(staffId, false, deactivatedBy))
                .Returns(Task.CompletedTask);

            // Act
            var result = await Service.DeactivateStaffMemberAsync(staffId, deactivatedBy);

            // Assert
            result.Should().BeTrue();
            MockRepository.Verify(
                r => r.SetActiveStatusAsync(staffId, false, deactivatedBy),
                Times.Once
            );
        }

        [Fact]
        public async Task ActivateStaffMemberAsync_ActivatesStaff()
        {
            // Arrange
            var staffId = Guid.NewGuid();
            var activatedBy = Guid.NewGuid();

            MockRepository
                .Setup(r => r.SetActiveStatusAsync(staffId, true, activatedBy))
                .Returns(Task.CompletedTask);

            // Act
            var result = await Service.ActivateStaffMemberAsync(staffId, activatedBy);

            // Assert
            result.Should().BeTrue();
            MockRepository.Verify(
                r => r.SetActiveStatusAsync(staffId, true, activatedBy),
                Times.Once
            );
        }

        [Theory]
        [InlineData("Security")]
        [InlineData("Plumber")]
        [InlineData("Electrician")]
        [InlineData("Carpenter")]
        public async Task CreateStaffMemberAsync_WithValidStaffTypes_CreatesSuccessfully(string staffType)
        {
            // Arrange
            var createdBy = Guid.NewGuid();
            var createDto = new CreateStaffMemberDto
            {
                FullName = $"Test {staffType}",
                Phone = "1234567890",
                StaffType = staffType
            };

            MockRepository
                .Setup(r => r.PhoneExistsAsync(createDto.Phone))
                .ReturnsAsync(false);

            MockRepository
                .Setup(r => r.CreateAsync(createDto, createdBy))
                .Returns(Task.CompletedTask);

            var createdStaff = new StaffMemberDto
            {
                StaffId = Guid.NewGuid(),
                FullName = createDto.FullName,
                Phone = createDto.Phone,
                StaffType = staffType,
                IsActive = true
            };

            MockRepository
                .Setup(r => r.GetByTypeAsync(staffType))
                .ReturnsAsync(new List<StaffMemberDto> { createdStaff });

            // Act
            var result = await Service.CreateStaffMemberAsync(createDto, createdBy);

            // Assert
            result.Should().NotBeNull();
            result.StaffType.Should().Be(staffType);
        }
    }
}