namespace ApartmentManagementSystem.Application.DTOs.Community
{
    public class AssignCommunityRoleRequestDto
    {
        public Guid UserId { get; set; }
        public string CommunityRole { get; set; } = string.Empty;
        public Guid? ApartmentId { get; set; }
    }
}