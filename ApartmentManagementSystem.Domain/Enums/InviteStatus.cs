using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Enums
{
   public static class InviteStatus
    {
        public const string Pending = "Pending";
        public const string OtpVerified = "OtpVerified";
        public const string Completed = "Completed";
        public const string Expired = "Expired";
        public const string Rejected = "Rejected";
    }
}