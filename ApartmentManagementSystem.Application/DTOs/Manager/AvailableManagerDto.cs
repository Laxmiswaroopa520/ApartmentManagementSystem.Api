namespace ApartmentManagementSystem.Application.DTOs.Manager
{
    public class AvailableManagerDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        //Flat number for residents
        public string? FlatNumber { get; set; }

        public bool IsCurrentlyAssigned { get; set; }
        public string? CurrentApartmentName { get; set; }
    }
}