//Track registration workflow status
namespace ApartmentManagementSystem.Domain.Enums
{
   public enum ResidentStatus
    {
        PendingOtpVerification = 1,
        PendingRegistrationCompletion = 2,
        PendingFlatAllocation = 3,
        Active = 4,
        Inactive = 5,
        Rejected = 6
    }
}