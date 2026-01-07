using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Domain.Entities
{
    public class UserOtp
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
        public string Otp { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool IsVerified { get; set; }
    }
}