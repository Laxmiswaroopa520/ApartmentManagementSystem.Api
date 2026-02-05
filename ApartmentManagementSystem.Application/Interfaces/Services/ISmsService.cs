
namespace ApartmentManagementSystem.Application.Interfaces.Services
{
     public interface ISmsService
        {
            Task SendAsync(string phone, string message);
        }
    }
