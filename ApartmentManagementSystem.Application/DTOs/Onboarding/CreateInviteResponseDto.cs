
//used in the ui for storing full name,mobile number and otp..
namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
   public class CreateInviteResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public string ResidentType { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }


}