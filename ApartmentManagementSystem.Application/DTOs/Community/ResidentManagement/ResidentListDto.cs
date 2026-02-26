namespace ApartmentManagementSystem.Application.DTOs.Community.ResidentManagement
{
    public class ResidentListDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ResidentType { get; set; } = string.Empty;
        public string? FlatNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RegisteredOn { get; set; }
        public string DisplayText { get; set; } = string.Empty;
    }
}
