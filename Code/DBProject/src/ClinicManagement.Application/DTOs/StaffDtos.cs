namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for staff display.
/// </summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Gender { get; set; }
    public string Designation { get; set; } = string.Empty;
    public double? Salary { get; set; }
}

/// <summary>
/// DTO for staff list view.
/// </summary>
public class StaffListDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>
/// DTO for adding new staff.
/// </summary>
public class AddStaffDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}
