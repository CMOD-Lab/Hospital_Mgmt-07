namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents other (non-doctor) staff members.
/// </summary>
public class OtherStaff
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Designation { get; set; } = string.Empty;
    public char Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? HighestQualification { get; set; }
    public double? Salary { get; set; }
}
