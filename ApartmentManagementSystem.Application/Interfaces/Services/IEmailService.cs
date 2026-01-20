using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IEmailService
    {
        //Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(string to, string subject, string body);
        Task SendWelcomeEmailAsync(string to, string fullName);
        Task SendRegistrationCompletedToAdminAsync(string residentName, string phone, string residentType);
        Task SendFlatAssignedToResidentAsync(string to, string residentName, string flatNumber);
    }
}*/
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);

        Task SendRegistrationCompletedToAdminAsync(
            string residentName,
            string phone,
            string residentType
        );

        Task SendFlatAssignedToResidentAsync(
            string to,
            string residentName,
            string flatNumber
        );

        Task SendWelcomeEmailAsync(
            string to,
            string fullName
        );
    }
}
