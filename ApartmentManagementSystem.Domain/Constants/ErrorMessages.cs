namespace ApartmentManagementSystem.Domain.Constants;
// Centralized error messages used throughout the application
public static class ErrorMessages
{
    // User-related errors
    public const string UserNotFound = "User not found";
    public const string PhoneAlreadyExists = "Phone number already exists";
    public const string UsernameAlreadyExists = "Username already exists";
    public const string EmailAlreadyExists = "Email already exists";
    public const string InvalidCredentials = "Invalid credentials";
    public const string AccountInactive = "Your account is inactive. Please contact the administrator.";

    // OTP-related errors
    public const string InvalidOtp = "Invalid or expired OTP";
    public const string OtpExpired = "OTP has expired";
    public const string OtpAlreadyUsed = "OTP has already been used";

    // Registration errors
    public const string RegistrationNotCompleted = "Registration not completed";
    public const string OtpNotVerified = "Please verify OTP first";

    // Flat/Apartment errors
    public const string FlatNotFound = "Flat not found";
    public const string FlatAlreadyOccupied = "Flat is already occupied";
    public const string ApartmentNotFound = "Apartment not found";

    // General errors
    public const string InvalidRequest = "Invalid request";
    public const string UnauthorizedAccess = "Unauthorized access";
    public const string OperationFailed = "Operation failed";
}