namespace ApartmentManagementSystem.Application.DTOs.Community
{
    public class AssignCommunityRoleDto
    {
        public Guid UserId { get; set; }
        public string CommunityRole { get; set; } = string.Empty; // President, Secretary, Treasurer
    }
}
