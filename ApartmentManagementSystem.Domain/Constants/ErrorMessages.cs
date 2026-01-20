using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagementSystem.Domain.Constants
{
  public  static class ErrorMessages
    {
        // Auth
        public const string InvalidCredentials = "Invalid username or password";
        public const string UserNotActive = "User account is not active";
        public const string RegistrationNotCompleted = "Please complete registration before logging in";
        public const string FlatNotAssigned = "Flat not assigned yet. Please contact administrator";

        // OTP
        public const string InvalidOtp = "Invalid or expired OTP";
        public const string OtpExpired = "OTP has expired";
        public const string OtpAlreadyUsed = "OTP has already been used";

        // Invite
        public const string InviteNotFound = "Invite not found";
        public const string PhoneAlreadyExists = "Phone number already registered";
        public const string UsernameAlreadyExists = "Username already taken";

        // Flat Assignment
        public const string FlatAlreadyOccupied = "This flat is already occupied";
        public const string FlatNotFound = "Flat not found";
        public const string UserNotFound = "User not found";
        public const string Unauthorized = "You are not authorized to perform this action";
    }
}