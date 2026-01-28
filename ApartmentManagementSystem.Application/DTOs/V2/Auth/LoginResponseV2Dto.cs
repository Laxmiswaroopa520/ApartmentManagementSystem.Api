using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs.V2.Auth
{
  
        public class LoginResponseV2Dto
        {
            public Guid UserId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public string AccessToken { get; set; } = string.Empty;

            // NEW in V2
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime AccessTokenExpiry { get; set; }
            public DateTime RefreshTokenExpiry { get; set; }
            public string TokenType { get; set; } = "Bearer";
        }

      

        
    }
