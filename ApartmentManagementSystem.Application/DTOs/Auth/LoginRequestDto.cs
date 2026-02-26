using System.ComponentModel.DataAnnotations;
namespace ApartmentManagementSystem.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
       // LoginRequestDto.cs 
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; } = string.Empty;
        
    }
}
