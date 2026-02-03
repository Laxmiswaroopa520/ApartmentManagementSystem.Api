using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
