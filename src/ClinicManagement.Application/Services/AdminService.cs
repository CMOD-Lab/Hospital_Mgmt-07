using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service for admin operations.</summary>
public class AdminService
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IDepartmentRepository _deptRepo;
    private readonly ILoginRepository _loginRepo;
    private readonly IEmailService _emailService;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepo,
        IStaffRepository staffRepo,
        IPatientRepository patientRepo,
        IAppointmentRepository appointmentRepo,
        IDepartmentRepository deptRepo,
        ILoginRepository loginRepo,
        IEmailService emailService,
        ILogger<AdminService> logger)
    {
        _doctorRepo = doctorRepo;
        _staffRepo = staffRepo;
        _patientRepo = patientRepo;
        _appointmentRepo = appointmentRepo;
        _deptRepo = deptRepo;
        _loginRepo = loginRepo;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>Gets admin home statistics.</summary>
    public virtual async Task<AdminHomeDto> GetAdminHomeInfoAsync(CancellationToken ct = default)
    {
        try
        {
            var doctors = await _doctorRepo.GetAllActiveAsync(ct);
            var patients = await _patientRepo.GetAllAsync(ct);
            var departments = await _deptRepo.GetAllAsync(ct);

            var deptStats = departments.Select(d => new DepartmentStatsDto
            {
                DeptName = d.DeptName,
                DoctorCount = doctors.Count(doc => doc.DeptNo == d.DeptNo),
                PatientCount = 0
            }).ToList();

            double totalIncome = doctors.Sum(d => d.PatientsTreated * d.ChargesPerVisit);

            return new AdminHomeDto
            {
                TotalDoctors = doctors.Count(),
                TotalPatients = patients.Count(),
                TotalIncome = totalIncome,
                DepartmentStats = deptStats
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting admin home info");
            return new AdminHomeDto();
        }
    }

    /// <summary>Gets all doctors with optional search.</summary>
    public virtual async Task<IEnumerable<DoctorListDto>> GetDoctorsAsync(string searchQuery = "", CancellationToken ct = default)
    {
        try
        {
            IEnumerable<Doctor> doctors;
            if (string.IsNullOrWhiteSpace(searchQuery))
                doctors = await _doctorRepo.GetAllActiveAsync(ct);
            else
                doctors = await _doctorRepo.SearchAsync(searchQuery, ct);

            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                DepartmentName = d.Department?.DeptName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors list");
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>Gets all patients with optional search.</summary>
    public virtual async Task<IEnumerable<PatientListDto>> GetPatientsAsync(string searchQuery = "", CancellationToken ct = default)
    {
        try
        {
            IEnumerable<Patient> patients;
            if (string.IsNullOrWhiteSpace(searchQuery))
                patients = await _patientRepo.GetAllAsync(ct);
            else
                patients = await _patientRepo.SearchAsync(searchQuery, ct);

            return patients.Select(p => new PatientListDto
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Phone = p.Phone
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patients list");
            return Enumerable.Empty<PatientListDto>();
        }
    }

    /// <summary>Gets all staff with optional search.</summary>
    public virtual async Task<IEnumerable<StaffListDto>> GetStaffAsync(string searchQuery = "", CancellationToken ct = default)
    {
        try
        {
            IEnumerable<OtherStaff> staff;
            if (string.IsNullOrWhiteSpace(searchQuery))
                staff = await _staffRepo.GetAllAsync(ct);
            else
                staff = await _staffRepo.SearchAsync(searchQuery, ct);

            return staff.Select(s => new StaffListDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Designation = s.Designation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff list");
            return Enumerable.Empty<StaffListDto>();
        }
    }

    /// <summary>Adds a new doctor.</summary>
    public virtual async Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken ct = default)
    {
        try
        {
            var emailExists = await _doctorRepo.EmailExistsAsync(dto.Email, ct);
            if (emailExists) return false;

            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = UserType.Doctor
            };
            var loginId = await _loginRepo.AddAsync(login, ct);

            if (!DateOnly.TryParse(dto.BirthDate, out var birthDate))
                return false;

            var doctor = new Doctor
            {
                DoctorId = loginId,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = birthDate,
                Gender = dto.Gender,
                DeptNo = dto.DeptNo,
                ChargesPerVisit = dto.ChargesPerVisit,
                MonthlySalary = dto.Salary,
                Qualification = dto.Qualification,
                Specialization = dto.Specialization,
                WorkExperience = dto.Experience,
                Status = 1
            };

            await _doctorRepo.AddAsync(doctor, ct);
            _logger.LogInformation("Doctor {Name} added successfully", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>Adds new staff member.</summary>
    public virtual async Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken ct = default)
    {
        try
        {
            DateOnly? birthDate = null;
            if (DateOnly.TryParse(dto.BirthDate, out var bd))
                birthDate = bd;

            var staff = new OtherStaff
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                Gender = dto.Gender,
                Designation = dto.Designation,
                BirthDate = birthDate,
                HighestQualification = dto.Qualification,
                Salary = dto.Salary
            };

            await _staffRepo.AddAsync(staff, ct);
            _logger.LogInformation("Staff {Name} added successfully", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>Soft-deletes a doctor (sets status to 0).</summary>
    public virtual async Task<bool> DeleteDoctorAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await _doctorRepo.SoftDeleteAsync(id, ct);
            _logger.LogInformation("Doctor {Id} soft-deleted", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor {Id}", id);
            return false;
        }
    }

    /// <summary>Deletes a staff member.</summary>
    public virtual async Task<bool> DeleteStaffAsync(int id, CancellationToken ct = default)
    {
        try
        {
            await _staffRepo.DeleteAsync(id, ct);
            _logger.LogInformation("Staff {Id} deleted", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff {Id}", id);
            return false;
        }
    }

    /// <summary>Gets patient details by ID.</summary>
    public virtual async Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var patient = await _patientRepo.GetByIdAsync(id, ct);
            if (patient == null) return null;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - patient.BirthDate.Year;
            if (patient.BirthDate > today.AddYears(-age)) age--;

            return new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Phone = patient.Phone,
                Address = patient.Address,
                BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
                Age = age,
                Gender = patient.Gender.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient {Id}", id);
            return null;
        }
    }

    /// <summary>Gets staff details by ID.</summary>
    public virtual async Task<StaffDto?> GetStaffByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var staff = await _staffRepo.GetByIdAsync(id, ct);
            if (staff == null) return null;

            return new StaffDto
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Phone = staff.Phone,
                Address = staff.Address,
                Gender = staff.Gender.ToString(),
                Designation = staff.Designation,
                Salary = (int)(staff.Salary ?? 0)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff {Id}", id);
            return null;
        }
    }

    /// <summary>Gets all departments.</summary>
    public virtual async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken ct = default)
    {
        try
        {
            var depts = await _deptRepo.GetAllAsync(ct);
            return depts.Select(d => new DepartmentDto
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName,
                Description = d.Description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }

    /// <summary>Marks a doctor as out-of-office for a specific date and notifies all scheduled patients.</summary>
    public virtual async Task<NotificationSummaryDto> MarkDoctorOutOfOfficeAsync(int doctorId, DateOnly outOfOfficeDate, CancellationToken ct = default)
    {
        var summary = new NotificationSummaryDto();

        try
        {
            // Get doctor information
            var doctor = await _doctorRepo.GetByIdAsync(doctorId, ct);
            if (doctor == null)
            {
                _logger.LogWarning("Doctor {DoctorId} not found", doctorId);
                return summary;
            }

            // Get all appointments for the doctor on the specified date with status Pending or Approved
            var appointments = await _appointmentRepo.GetByDoctorIdAndDateAsync(doctorId, outOfOfficeDate, ct);

            if (!appointments.Any())
            {
                _logger.LogInformation("No appointments found for doctor {DoctorId} on {Date}", doctorId, outOfOfficeDate);
                return summary;
            }

            // Send email notification to each patient
            foreach (var appointment in appointments)
            {
                try
                {
                    // Skip if patient or login information is missing
                    if (appointment.Patient == null || appointment.Patient.Login == null)
                    {
                        _logger.LogWarning("Patient or login information missing for appointment {AppointmentId}", appointment.AppointId);
                        if (appointment.PatientId.HasValue)
                        {
                            summary.FailureCount++;
                            summary.FailedPatientIds.Add(appointment.PatientId.Value);
                        }
                        continue;
                    }

                    var patientEmail = appointment.Patient.Login.Email;
                    if (string.IsNullOrWhiteSpace(patientEmail))
                    {
                        _logger.LogWarning("Email address missing for patient {PatientId} in appointment {AppointmentId}",
                            appointment.PatientId, appointment.AppointId);
                        if (appointment.PatientId.HasValue)
                        {
                            summary.FailureCount++;
                            summary.FailedPatientIds.Add(appointment.PatientId.Value);
                        }
                        continue;
                    }

                    // Send notification email
                    var appointmentDateTime = appointment.Date ?? DateTime.MinValue;
                    var success = await _emailService.SendOutOfOfficeNotificationAsync(
                        patientEmail,
                        doctor.Name,
                        appointmentDateTime,
                        "804-243-4668",
                        ct);

                    if (success)
                    {
                        summary.SuccessCount++;
                        _logger.LogInformation("Successfully notified patient {PatientId} for appointment {AppointmentId}",
                            appointment.PatientId, appointment.AppointId);
                    }
                    else
                    {
                        summary.FailureCount++;
                        if (appointment.PatientId.HasValue)
                        {
                            summary.FailedPatientIds.Add(appointment.PatientId.Value);
                        }
                        _logger.LogError("Failed to send notification to patient {PatientId} for appointment {AppointmentId}",
                            appointment.PatientId, appointment.AppointId);
                    }
                }
                catch (Exception ex)
                {
                    summary.FailureCount++;
                    if (appointment.PatientId.HasValue)
                    {
                        summary.FailedPatientIds.Add(appointment.PatientId.Value);
                    }
                    _logger.LogError(ex, "Error sending notification to patient {PatientId} for appointment {AppointmentId}",
                        appointment.PatientId, appointment.AppointId);
                }
            }

            _logger.LogInformation("Out-of-office notifications completed for doctor {DoctorId} on {Date}. Success: {SuccessCount}, Failures: {FailureCount}",
                doctorId, outOfOfficeDate, summary.SuccessCount, summary.FailureCount);

            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing out-of-office notifications for doctor {DoctorId} on {Date}", doctorId, outOfOfficeDate);
            return summary;
        }
    }
}
