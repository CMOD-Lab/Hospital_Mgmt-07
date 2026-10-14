namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents a patient in the clinic management system.
/// </summary>
public class Patient
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public char Gender { get; set; }

    // Navigation properties
    public LoginTable? Login { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
