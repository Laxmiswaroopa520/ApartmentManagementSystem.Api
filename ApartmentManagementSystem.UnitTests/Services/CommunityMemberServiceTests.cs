// ApartmentManagementSystem.UnitTests/Services/CommunityMemberServiceTests.cs
using ApartmentManagementSystem.Application.DTOs.Community;
using ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement;
using ApartmentManagementSystem.Application.Interfaces.Repositories;
using ApartmentManagementSystem.Application.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApartmentManagementSystem.UnitTests.Services
{
    public class CommunityMemberServiceTests
    {
        private readonly Mock<ICommunityMemberRepository> MockRepository;
        private readonly CommunityMemberService CommunityService;

        public CommunityMemberServiceTests()
        {
            MockRepository = new Mock<ICommunityMemberRepository>();
            CommunityService = new CommunityMemberService(MockRepository.Object);
        }

        [Fact]
        public async Task GetAllCommunityMembersAsync_WithoutApartmentId_ReturnsAllMembers()
        {
            // Arrange
            var apartmentId1 = Guid.NewGuid();
            var apartmentId2 = Guid.NewGuid();

            var members = new List<CommunityMemberDto>
            {
                new CommunityMemberDto
                {
                    UserId = Guid.NewGuid(),
                    FullName = "John President",
                    Role = "President",
                    ApartmentId = apartmentId1,
                    IsActive = true
                },
                new CommunityMemberDto
                {
                    UserId = Guid.NewGuid(),
                    FullName = "Jane Secretary",
                    Role = "Secretary",
                    ApartmentId = apartmentId2,
                    IsActive = true
                }
            };

            MockRepository
                .Setup(r => r.GetAllCommunityMembersAsync())
                .ReturnsAsync(members);

            // Act
            var result = await CommunityService.GetAllCommunityMembersAsync(null);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(members);
        }

        [Fact]
        public async Task GetAllCommunityMembersAsync_WithApartmentId_ReturnsFilteredMembers()
        {
            // Arrange
            var apartmentId1 = Guid.NewGuid();
            var apartmentId2 = Guid.NewGuid();

            var allMembers = new List<CommunityMemberDto>
            {
                new CommunityMemberDto
                {
                    UserId = Guid.NewGuid(),
                    FullName = "John President",
                    Role = "President",
                    ApartmentId = apartmentId1,
                    IsActive = true
                },
                new CommunityMemberDto
                {
                    UserId = Guid.NewGuid(),
                    FullName = "Jane Secretary",
                    Role = "Secretary",
                    ApartmentId = apartmentId2,
                    IsActive = true
                }
            };

            MockRepository
                .Setup(r => r.GetAllCommunityMembersAsync())
                .ReturnsAsync(allMembers);

            // Act
            var result = await CommunityService.GetAllCommunityMembersAsync(apartmentId1);

            // Assert
            result.Should().HaveCount(1);
            result.First().ApartmentId.Should().Be(apartmentId1);
            result.First().FullName.Should().Be("John President");
        }

        [Fact]
        public async Task GetEligibleResidentsForApartmentAsync_ReturnsEligibleResidents()
        {
            // Arrange
            var apartmentId = Guid.NewGuid();
            var eligibleResidents = new List<ResidentListDto>
            {
                new ResidentListDto
                {
                    UserId = Guid.NewGuid(),
                    FullName = "Alice Smith",
                    ResidentType = "Owner",
                    FlatNumber = "101"
                },
                new ResidentListDto
                {
                    UserId = Guid.NewGuid(),
                    FullName = "Bob Jones",
                    ResidentType = "Owner",
                    FlatNumber = "102"
                }
            };

            MockRepository
                .Setup(r => r.GetEligibleResidentsForApartmentAsync(apartmentId))
                .ReturnsAsync(eligibleResidents);

            // Act
            var result = await CommunityService.GetEligibleResidentsForApartmentAsync(apartmentId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(eligibleResidents);
        }

        [Fact]
        public async Task AssignCommunityRoleAsync_WithValidData_AssignsRole()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();
            var assignedBy = Guid.NewGuid();
            var roleName = "President";

            MockRepository
                .Setup(r => r.CommunityRoleExistsForApartmentAsync(roleName, apartmentId))
                .ReturnsAsync(false);

            MockRepository
                .Setup(r => r.AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy))
                .Returns(Task.CompletedTask);

            var memberDto = new CommunityMemberDto
            {
                UserId = userId,
                FullName = "John Doe",
                Role = roleName,
                ApartmentId = apartmentId,
                IsActive = true
            };

            MockRepository
                .Setup(r => r.GetCommunityMemberByUserIdAsync(userId))
                .ReturnsAsync(memberDto);

            // Act
            var result = await CommunityService.AssignCommunityRoleAsync(
                userId, roleName, apartmentId, assignedBy
            );

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Role.Should().Be(roleName);
            result.ApartmentId.Should().Be(apartmentId);

            MockRepository.Verify(
                r => r.AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy),
                Times.Once
            );
        }

        [Fact]
        public async Task AssignCommunityRoleAsync_WhenRoleAlreadyExists_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();
            var assignedBy = Guid.NewGuid();
            var roleName = "President";

            MockRepository
                .Setup(r => r.CommunityRoleExistsForApartmentAsync(roleName, apartmentId))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => CommunityService.AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy)
            );

            exception.Message.Should().Contain("already assigned");
        }

        [Fact]
        public async Task AssignCommunityRoleAsync_WhenMemberNotRetrieved_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var apartmentId = Guid.NewGuid();
            var assignedBy = Guid.NewGuid();
            var roleName = "Secretary";

            MockRepository
                .Setup(r => r.CommunityRoleExistsForApartmentAsync(roleName, apartmentId))
                .ReturnsAsync(false);

            MockRepository
                .Setup(r => r.AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy))
                .Returns(Task.CompletedTask);

            MockRepository
                .Setup(r => r.GetCommunityMemberByUserIdAsync(userId))
                .ReturnsAsync((CommunityMemberDto?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => CommunityService.AssignCommunityRoleAsync(userId, roleName, apartmentId, assignedBy)
            );

            exception.Message.Should().Contain("Failed to retrieve");
        }

        [Fact]
        public async Task RemoveCommunityRoleAsync_RemovesRole()
        {
            // Arrange
            var userId = Guid.NewGuid();

            MockRepository
                .Setup(r => r.RemoveCommunityRoleAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            await CommunityService.RemoveCommunityRoleAsync(userId);

            // Assert
            MockRepository.Verify(
                r => r.RemoveCommunityRoleAsync(userId),
                Times.Once
            );
        }

        [Fact]
        public async Task GetCommunityMemberByUserIdAsync_ReturnsMember()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var memberDto = new CommunityMemberDto
            {
                UserId = userId,
                FullName = "Test User",
                Role = "Treasurer",
                IsActive = true
            };

            MockRepository
                .Setup(r => r.GetCommunityMemberByUserIdAsync(userId))
                .ReturnsAsync(memberDto);

            // Act
            var result = await CommunityService.GetCommunityMemberByUserIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(memberDto);
        }
    }
}