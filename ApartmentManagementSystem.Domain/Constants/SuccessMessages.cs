using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Constants
{
 public   static class SuccessMessages
    {
        public const string LoginSuccess = "Login successful";
        public const string InviteCreated = "User invited successfully";
        public const string OtpVerified = "OTP verified successfully";
        public const string RegistrationCompleted = "Registration completed successfully. Awaiting flat assignment";
        public const string LogoutSuccess = "Logged out successfully";
        public const string FlatAssigned = "Flat assigned successfully";
    }
}