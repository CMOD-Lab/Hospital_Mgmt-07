namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents a login entry in the system.
/// Type: 1 = Patient, 2 = Doctor, 3 = Admin
/// </summary>
public class LoginTable
{
    public int LoginId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Type { get; set; }

    // Navigation properties
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}
