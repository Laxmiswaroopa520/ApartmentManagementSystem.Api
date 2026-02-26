using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
namespace ApartmentManagementSystem.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string phone, string message)
        {
            _logger.LogInformation($"[DEV SMS] {phone}: {message}");
            return Task.CompletedTask;
        }
    }
}