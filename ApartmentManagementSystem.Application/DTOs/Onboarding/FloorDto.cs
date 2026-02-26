namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class FloorDto
    {
        public Guid Id { get; set; }
        public int FloorNumber { get; set; }
        public Guid ApartmentId { get; set; } 
        public string ApartmentName { get; set; } = string.Empty; 
    }
}
