namespace ClinicManagement.Application.DTOs;

/// <summary>Summary of notification results.</summary>
public class NotificationSummaryDto
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<int> FailedPatientIds { get; set; } = new();
}
