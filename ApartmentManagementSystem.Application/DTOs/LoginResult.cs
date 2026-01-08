using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Application.DTOs
{
    public class LoginResult
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}