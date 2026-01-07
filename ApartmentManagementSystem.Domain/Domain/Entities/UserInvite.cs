using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Domain.Entities
{
    public class UserInvite
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
        public string Mobile { get; set; }

        public Guid RoleId { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}