namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class CompleteRegistrationResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "PendingFlatAllocation"
    }
}