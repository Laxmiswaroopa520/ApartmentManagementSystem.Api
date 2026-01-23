namespace ApartmentManagementSystem.Domain.Entities
{
    public class Flat
    {
        public Guid Id { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public string Name { get; set; } = null!;
       
        // FOREIGN KEYS
  
        public Guid ApartmentId { get; set; }
        public Guid FloorId { get; set; }

        public Guid? OwnerUserId { get; set; }   // NULL = available

       
        // NAVIGATION PROPERTIES
        public Apartment? Apartment { get; set; } = null!;
        //  public Floor Floor { get; set; } = null!;

        public User? OwnerUser { get; set; }

        // Tenants via mapping table
        public ICollection<UserFlatMapping> UserFlatMappings { get; set; }
            = new List<UserFlatMapping>();
       
        // SYSTEM FIELDS
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Floor Floor { get; set; } = null!;
        public bool IsOccupied { get; set; }
    }
}







