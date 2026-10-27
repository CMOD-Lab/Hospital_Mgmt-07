namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for patient information display.</summary>
public class PatientDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? BirthDate { get; set; }
    public int Age { get; set; }
    public string? Gender { get; set; }
}

/// <summary>DTO for patient list display.</summary>
public class PatientListDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
