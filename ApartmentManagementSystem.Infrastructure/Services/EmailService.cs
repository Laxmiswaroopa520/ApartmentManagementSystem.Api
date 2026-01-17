using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ApartmentManagementSystem.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string to, string subject, string body)
        {
            _logger.LogInformation($"[DEV EMAIL] To:{to}, Subject:{subject}");
            return Task.CompletedTask;
        }
    }
}
