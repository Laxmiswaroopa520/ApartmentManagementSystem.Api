namespace ApartmentManagementSystem.Application.DTOs.Apartment
{
    public class ApartmentDiagramDto
    {
        public Guid ApartmentId { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty; //Added this for visualizing 3d view of my apartments..
        public int TotalFloors { get; set; }
        public List<FloorDiagramDto> Floors { get; set; } = new();
    }
}