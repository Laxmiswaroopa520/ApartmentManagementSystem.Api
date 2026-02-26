namespace ApartmentManagementSystem.Application.DTOs.Apartment
{
    public class CreateApartmentDto
    {
        public string Name { get; set; } = string.Empty; // Block/Building Name
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }

        public int TotalFloors { get; set; }
        public int FlatsPerFloor { get; set; }

        //  Custom flat numbering pattern
        public string? FlatNumberingPattern { get; set; } // e.g., "101, 102..." or "A1, A2..."
    }
}
