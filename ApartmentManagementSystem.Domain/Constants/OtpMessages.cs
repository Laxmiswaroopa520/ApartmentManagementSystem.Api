namespace ApartmentManagementSystem.Domain.Constants
{
    /// <summary>
    /// Contains constant messages related to OTP operations.
    /// </summary>
    public static class OtpMessages
    {
        /// <summary>
        /// Swagger description for OTP verification endpoint.
        /// </summary>
        public const string VerifyOtpDescription = @"
            Verifies the OTP code sent to the resident's phone number.
            OTP is valid for 10 minutes from generation.
            After successful verification, user can proceed to complete registration.
        ";

        /// <summary>
        /// Message returned when OTP verification succeeds.
        /// </summary>
        public const string OtpVerifiedSuccessfully =
            "OTP verified successfully";
        public const string OtpNotVerified = "Please verify OTP first";
    }
}