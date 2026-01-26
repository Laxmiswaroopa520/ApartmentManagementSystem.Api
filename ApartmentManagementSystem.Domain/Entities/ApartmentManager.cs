namespace ApartmentManagementSystem.Domain.Entities
{
    public class ApartmentManager
    {
        public Guid Id { get; set; }
        public Guid ApartmentId { get; set; }
        public Guid UserId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public Guid AssignedBy { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public Apartment Apartment { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}