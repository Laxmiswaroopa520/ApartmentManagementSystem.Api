

using System;

namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
    public class CompleteRegistrationDto
    {
        public string PrimaryPhone { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? SecondaryPhone { get; set; }
        public string? Email { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // ✅ NEW
        public Guid? FloorId { get; set; }
        public Guid? FlatId { get; set; }
    }
}
