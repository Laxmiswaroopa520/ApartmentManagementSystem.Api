namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class FlatDto
    {
        public Guid Id { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public Guid FloorId { get; set; }
        public Guid ApartmentId { get; set; } 
        public bool IsOccupied { get; set; }
    }
}

