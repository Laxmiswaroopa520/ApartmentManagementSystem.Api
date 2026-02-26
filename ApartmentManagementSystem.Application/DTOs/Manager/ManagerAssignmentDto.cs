namespace ApartmentManagementSystem.Application.DTOs.Manager
{
    public class ManagerAssignmentDto
    {
        public Guid ApartmentId { get; set; }
        public string ApartmentName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}