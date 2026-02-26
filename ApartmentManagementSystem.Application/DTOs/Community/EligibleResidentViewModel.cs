namespace ApartmentManagementSystem.Application.DTOs.Community
{

    public class EligibleResidentViewModel
    {
        public Guid UserId { get; set; }
        public string DisplayText { get; set; } = string.Empty; // e.g. "Ravi Kumar - Flat 301"
    }
}
