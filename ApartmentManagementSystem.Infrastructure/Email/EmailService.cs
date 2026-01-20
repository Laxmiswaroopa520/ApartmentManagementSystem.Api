// Infrastructure/Email/EmailService.cs
using System.Net;
using System.Net.Mail;
using ApartmentManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ApartmentManagementSystem.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? "";
        var smtpPass = _configuration["EmailSettings:SmtpPassword"] ?? "";
        var fromEmail = _configuration["EmailSettings:FromEmail"] ?? smtpUser;

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUser, smtpPass)
        };

        var message = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);

        try
        {
            await client.SendMailAsync(message); // Fixed: was SendAsync(message)
            _logger.LogInformation($"Email sent to {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Email failed: {ex.Message}");
        }
    }

    public async Task SendRegistrationCompletedToAdminAsync(string residentName, string phone, string residentType)
    {
        var adminEmail = _configuration["EmailSettings:AdminEmail"] ?? "admin@apartment.com";
        var subject = "New Resident Registration - Flat Assignment Required";
        var body = $@"
            <h2>New Resident Registered</h2>
            <p><strong>Name:</strong> {residentName}</p>
            <p><strong>Phone:</strong> {phone}</p>
            <p><strong>Type:</strong> {residentType}</p>
            <p><strong>Status:</strong> Pending Flat Allocation</p>
            <br/>
            <p>Please log in to admin panel to assign a flat.</p>
        ";

        await SendEmailAsync(adminEmail, subject, body);
    }

    public async Task SendFlatAssignedToResidentAsync(string to, string residentName, string flatNumber)
    {
        var subject = "Flat Assigned - Welcome to Your New Home!";
        var body = $@"
            <h2>Welcome {residentName}!</h2>
            <p>Your flat has been assigned:</p>
            <p><strong>Flat Number:</strong> {flatNumber}</p>
            <br/>
            <p>You can now login and access all features.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string to, string fullName)
    {
        var subject = "Welcome to Apartment Management System";
        var body = $@"
            <h2>Welcome {fullName}!</h2>
            <p>Your account has been created successfully.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }
}