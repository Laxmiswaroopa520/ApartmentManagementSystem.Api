namespace ApartmentManagementSystem.Domain.Entities
{
    //this is actually used for user and role mapping like 1 user can have multiple roles like that ..
    public class UserRole
    {
        public Guid Id;//added these two for manager reference
        public DateTime AssignedAt;//added these two 
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
