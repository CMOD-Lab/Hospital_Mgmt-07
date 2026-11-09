namespace ClinicManagement.Application.Interfaces;

/// <summary>Service interface for sending email notifications.</summary>
public interface IEmailService
{
    /// <summary>Sends out-of-office notification to a patient.</summary>
    /// <param name="email">Patient's email address.</param>
    /// <param name="doctorName">Doctor's full name.</param>
    /// <param name="appointmentDateTime">Appointment date and time.</param>
    /// <param name="contactNumber">Contact number for rescheduling.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if email sent successfully, false otherwise.</returns>
    Task<bool> SendOutOfOfficeNotificationAsync(
        string email,
        string doctorName,
        DateTime appointmentDateTime,
        string contactNumber,
        CancellationToken ct = default);
}
