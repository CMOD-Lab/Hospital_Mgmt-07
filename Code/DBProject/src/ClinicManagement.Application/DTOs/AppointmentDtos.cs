namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for appointment display.
/// </summary>
public class AppointmentDto
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public int? PatientId { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public string? Timings { get; set; }
    public int AppointmentStatus { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public int? FeedbackStatus { get; set; }
}

/// <summary>
/// DTO for creating a new appointment.
/// </summary>
public class CreateAppointmentDto
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int FreeSlotId { get; set; }
}

/// <summary>
/// DTO for updating appointment prescription.
/// </summary>
public class UpdatePrescriptionDto
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}

/// <summary>
/// DTO for admin home statistics.
/// </summary>
public class AdminHomeDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentViewDto> Departments { get; set; } = new List<DepartmentViewDto>();
    public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}

/// <summary>
/// DTO for department view.
/// </summary>
public class DepartmentViewDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DoctorCount { get; set; }
}

/// <summary>
/// DTO for free appointment slot.
/// </summary>
public class FreeSlotDto
{
    public int SlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
