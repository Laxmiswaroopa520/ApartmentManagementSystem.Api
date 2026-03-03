namespace ApartmentManagementSystem.Domain.Constants
{
    /// <summary>
    /// Contains constant messages related to resident types.
    /// </summary>
    public static class ResidentTypeMessages
    {
        /// <summary>
        /// Swagger description for resident types endpoint.
        /// </summary>
        public const string ResidentTypesDescription = @"
            Returns all available resident types in the system:
            - 1: Owner
            - 2: Tenant
            - 3: Staff
        ";
    }
}