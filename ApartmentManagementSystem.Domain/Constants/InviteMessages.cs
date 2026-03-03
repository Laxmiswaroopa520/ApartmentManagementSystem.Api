namespace ApartmentManagementSystem.Domain.Constants
{
    /// <summary>
    /// Contains constant messages related to resident invite operations.
    /// </summary>
    public static class InviteMessages
    {
        /// <summary>
        /// Message returned when invite is created successfully.
        /// </summary>
        public const string InviteCreatedSuccessfully =
            "Invite created successfully";

        /// <summary>
        /// Message returned when resident type is invalid.
        /// </summary>
        public const string InvalidResidentType =
            "Invalid resident type. Must be 1 (Owner), 2 (Tenant), or 3 (Staff)";
    }
}