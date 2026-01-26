// Domain/Entities/CommunityMember.cs (Updated)
namespace ApartmentManagementSystem.Domain.Entities
{
    public class CommunityMember
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ApartmentId { get; set; } // ⭐ NEW: Scoped to apartment
        public string CommunityRole { get; set; } = string.Empty; // President, Secretary, Treasurer

        public DateTime AssignedAt { get; set; }
        public Guid AssignedBy { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public User User { get; set; } = null!;
        public Apartment Apartment { get; set; } = null!;
    }
}