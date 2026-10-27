namespace ClinicManagement.Domain.Entities;

/// <summary>Represents a doctor in the clinic management system.</summary>
public class Doctor
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateOnly BirthDate { get; set; }
    public char Gender { get; set; }

    public int DeptNo { get; set; }
    public double ChargesPerVisit { get; set; }
    public double? MonthlySalary { get; set; }
    public double? ReputeIndex { get; set; }
    public int PatientsTreated { get; set; } = 0;

    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int? WorkExperience { get; set; }

    /// <summary>1 = Present, 0 = Left</summary>
    public int Status { get; set; } = 1;

    // Navigation properties
    public Department? Department { get; set; }
    public LoginTable? Login { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
