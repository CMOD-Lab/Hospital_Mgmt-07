using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for appointment display.</summary>
public class AppointmentDto
{
    public int AppointId { get; set; }
    public string? DoctorName { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public string? Timings { get; set; }
    public AppointmentStatus Status { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>DTO for free appointment slots.</summary>
public class FreeSlotDto
{
    public int SlotId { get; set; }
    public string? Timings { get; set; }
}

/// <summary>DTO for updating prescription.</summary>
public class UpdatePrescriptionDto
{
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}

/// <summary>DTO for admin home statistics.</summary>
public class AdminHomeDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentStatsDto> DepartmentStats { get; set; } = new List<DepartmentStatsDto>();
    public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}

/// <summary>DTO for department statistics.</summary>
public class DepartmentStatsDto
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int PatientCount { get; set; }
}
