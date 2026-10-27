namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for doctor profile display.</summary>
public class DoctorProfileDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public double ChargesPerVisit { get; set; }
    public double ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
    public int WorkExperience { get; set; }
    public int Age { get; set; }
    public string? DepartmentName { get; set; }
}

/// <summary>DTO for doctor list display.</summary>
public class DoctorListDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DepartmentName { get; set; }
}

/// <summary>DTO for adding a new doctor.</summary>
public class AddDoctorDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
}
