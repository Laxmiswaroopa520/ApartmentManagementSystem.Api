namespace ApartmentManagementSystem.Application.DTOs.Onboarding
{
  public class VerifyOtpDto
    {
        public string PrimaryPhone { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
    }
}