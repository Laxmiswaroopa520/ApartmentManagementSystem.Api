namespace ApartmentManagementSystem.API.DTOs.Auth
{
    //This represents what MVC receives from API
    public class LoginApiResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}