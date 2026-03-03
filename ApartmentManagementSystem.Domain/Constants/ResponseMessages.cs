//for apartmentManagementApiController code
/*namespace ApartmentManagementSystem.Domain.Constants
{   
        public static class ResponseMessages
        {
            public const string ApartmentCreated = "Apartment created successfully with all floors and flats";
            public const string ApartmentsRetrieved = "Apartments retrieved successfully";
            public const string ApartmentDetailsRetrieved = "Apartment details retrieved successfully";
            public const string ApartmentDiagramGenerated = "Apartment diagram generated successfully";
            public const string ManagerAssigned = "Manager assigned successfully";

            public const string ApartmentNotFound = "Apartment not found";
        } 
}
*/

namespace ApartmentManagementSystem.Domain.Constants
{
    public static class ResponseMessages
    {
        // Apartment CRUD
        public const string ApartmentCreated = "Apartment created successfully with all floors and flats";
        public const string ApartmentsRetrieved = "Apartments retrieved successfully";
        public const string ApartmentDetailsRetrieved = "Apartment details retrieved successfully";
        public const string ApartmentDiagramGenerated = "Apartment diagram generated successfully";
        public const string ApartmentDeleted = "Apartment deleted successfully";   
        public const string ApartmentDeactivated = "Apartment deactivated successfully"; 

        // Manager
        public const string ManagerAssigned = "Manager assigned successfully";
        public const string ManagerRemoved = "Manager removed successfully";     

        // Not found / errors
        public const string ApartmentNotFound = "Apartment not found";
        public const string ApartmentHasOccupants = "Cannot delete apartment: one or more flats are currently occupied. Please vacate all flats before deleting."; 
        public const string NoFloorsFound = "No floors found for this apartment. Please ensure floors were created properly.";       
        public const string DiagramGenerationFailed = "Failed to generate apartment diagram - no floors data available";                   
    }
}