using ClinicManagement.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace ClinicManagement.Infrastructure.Services;

/// <summary>Email service implementation using System.Net.Mail.SmtpClient.</summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromAddress;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Validate and load SMTP configuration
        var smtpSection = _configuration.GetSection("SmtpSettings");
        _smtpHost = smtpSection["Host"] ?? throw new ArgumentException("SmtpSettings:Host is missing in configuration");

        if (!int.TryParse(smtpSection["Port"], out _smtpPort))
            throw new ArgumentException("SmtpSettings:Port is missing or invalid in configuration");

        _smtpUsername = smtpSection["Username"] ?? throw new ArgumentException("SmtpSettings:Username is missing in configuration");
        _smtpPassword = smtpSection["Password"] ?? throw new ArgumentException("SmtpSettings:Password is missing in configuration");
        _fromAddress = smtpSection["FromAddress"] ?? throw new ArgumentException("SmtpSettings:FromAddress is missing in configuration");
        _fromName = smtpSection["FromName"] ?? throw new ArgumentException("SmtpSettings:FromName is missing in configuration");

        if (!bool.TryParse(smtpSection["EnableSsl"], out _enableSsl))
            _enableSsl = true; // Default to true for security
    }

    /// <summary>Sends out-of-office notification email to a patient.</summary>
    public async Task<bool> SendOutOfOfficeNotificationAsync(
        string email,
        string doctorName,
        DateTime appointmentDateTime,
        string contactNumber,
        CancellationToken ct = default)
    {
        try
        {
            var subject = "Important: Doctor Out of Office - Appointment Rescheduling Required";
            var body = $@"Dear Patient,

We apologize for the inconvenience, but we must inform you that Dr. {doctorName} will be out of office on your scheduled appointment date.

Your Appointment Details:
- Doctor: Dr. {doctorName}
- Date and Time: {appointmentDateTime:yyyy-MM-dd HH:mm}

Please contact us at your earliest convenience to reschedule your appointment.

Contact Number: {contactNumber}

We apologize for any inconvenience this may cause and look forward to serving you at a rescheduled time.

Best regards,
Clinic Management Team";

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromAddress, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = _enableSsl
            };

            await smtpClient.SendMailAsync(mailMessage, ct);
            _logger.LogInformation("Email sent successfully to {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", email);
            return false;
        }
    }
}
