namespace ApartmentManagementSystem.Application.DTOs.Apartment
{
    public class CreateApartmentResponseDto
    {
        public Guid ApartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalFloors { get; set; }
        public int TotalFlats { get; set; }
        public List<FloorCreatedDto> FloorsCreated { get; set; } = new();
    }
}