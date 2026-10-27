using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities;

/// <summary>Represents a clinic appointment between a doctor and a patient.</summary>
public class Appointment
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public DateTime? Date { get; set; }
    public AppointmentStatus AppointmentStatus { get; set; } = AppointmentStatus.Pending;

    public double? BillAmount { get; set; }
    public BillStatus? BillStatus { get; set; }

    public NotificationStatus DoctorNotification { get; set; } = NotificationStatus.Unseen;
    public NotificationStatus PatientNotification { get; set; } = NotificationStatus.Unseen;
    public FeedbackStatus FeedbackStatus { get; set; } = FeedbackStatus.Pending;

    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
}
