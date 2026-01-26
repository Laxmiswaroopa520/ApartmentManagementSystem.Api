// Domain/Entities/Apartment.cs
using ApartmentManagementSystem.Domain.Enums;

namespace ApartmentManagementSystem.Domain.Entities
{
    public class Apartment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // Block Name
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }

        // Building Configuration
        public int TotalFloors { get; set; }
        public int FlatsPerFloor { get; set; }
        public int TotalFlats { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public ApartmentStatus Status { get; set; } = ApartmentStatus.UnderConstruction;

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        // Navigation Properties
        public ICollection<Floor> Floors { get; set; } = new List<Floor>();
        public ICollection<Flat> Flats { get; set; } = new List<Flat>();
        public ICollection<ApartmentManager> Managers { get; set; } = new List<ApartmentManager>();
        public ICollection<CommunityMember> CommunityMembers { get; set; } = new List<CommunityMember>();
    }
}













/*namespace ApartmentManagementSystem.Domain.Entities
{

    public class Apartment
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int TotalFlats { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Flat> Flats { get; set; } = new List<Flat>();
    }
}
*/