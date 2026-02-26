namespace ApartmentManagementSystem.Application.DTOs.Apartment
{
    public class FloorDiagramDto
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<FlatDiagramDto> Flats { get; set; } = new();
    }
}