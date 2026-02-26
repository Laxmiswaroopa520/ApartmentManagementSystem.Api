namespace ApartmentManagementSystem.Application.DTOs.Admin
{
   public class PendingResidentDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ResidentType { get; set; } = string.Empty;
        public DateTime RegisteredOn { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
