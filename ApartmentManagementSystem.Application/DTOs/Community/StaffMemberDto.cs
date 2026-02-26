namespace ApartmentManagementSystem.Application.DTOs.Community
{
    public class StaffMemberDto
    {
        public Guid StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string StaffType { get; set; } = string.Empty; // Plumber, Security, etc.
        public DateTime JoinedOn { get; set; }
        public bool IsActive { get; set; }
        public string? Specialization { get; set; }
        public decimal? HourlyRate { get; set; }
    }

}
