using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service for patient-facing operations.</summary>
public class PatientService
{
    private readonly IPatientRepository _patientRepo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IDoctorRepository _doctorRepo;
    private readonly IDepartmentRepository _deptRepo;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepo,
        IAppointmentRepository appointmentRepo,
        IDoctorRepository doctorRepo,
        IDepartmentRepository deptRepo,
        ILogger<PatientService> logger)
    {
        _patientRepo = patientRepo;
        _appointmentRepo = appointmentRepo;
        _doctorRepo = doctorRepo;
        _deptRepo = deptRepo;
        _logger = logger;
    }

    /// <summary>Gets patient profile information.</summary>
    public virtual async Task<PatientDto?> GetPatientInfoAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var patient = await _patientRepo.GetByIdAsync(patientId, ct);
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
            _logger.LogError(ex, "Error getting patient info for {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets bill history for a patient.</summary>
    public virtual async Task<IEnumerable<AppointmentDto>> GetBillHistoryAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetBillHistoryByPatientAsync(patientId, ct);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                DoctorName = a.Doctor?.Name,
                Date = a.Date,
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus?.ToString(),
                Status = a.AppointmentStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill history for patient {PatientId}", patientId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>Gets current appointment for a patient.</summary>
    public virtual async Task<AppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetCurrentByPatientAsync(patientId, ct);
            if (appointment == null) return null;

            return new AppointmentDto
            {
                AppointId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name,
                Date = appointment.Date,
                Timings = appointment.Date?.ToString("hh:mm tt"),
                Status = appointment.AppointmentStatus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets treatment history for a patient.</summary>
    public virtual async Task<IEnumerable<AppointmentDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetTreatmentHistoryByPatientAsync(patientId, ct);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                DoctorName = a.Doctor?.Name,
                Date = a.Date,
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription,
                Status = a.AppointmentStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting treatment history for patient {PatientId}", patientId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>Gets all departments for appointment booking.</summary>
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

    /// <summary>Gets doctors by department name.</summary>
    public virtual async Task<IEnumerable<DoctorListDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken ct = default)
    {
        try
        {
            var doctors = await _doctorRepo.GetByDepartmentAsync(deptName, ct);
            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                DepartmentName = d.Department?.DeptName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors for department {DeptName}", deptName);
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>Gets doctor profile.</summary>
    public virtual async Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var doctor = await _doctorRepo.GetByIdAsync(doctorId, ct);
            if (doctor == null) return null;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - doctor.BirthDate.Year;
            if (doctor.BirthDate > today.AddYears(-age)) age--;

            return new DoctorProfileDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender.ToString(),
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex ?? 0,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience ?? 0,
                Age = age,
                DepartmentName = doctor.Department?.DeptName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor profile {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>Gets free appointment slots for a doctor.</summary>
    public virtual async Task<IEnumerable<AppointmentDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken ct = default)
    {
        try
        {
            var slots = await _appointmentRepo.GetFreeSlotsByDoctorAsync(doctorId, patientId, ct);
            return slots.Select(s => new AppointmentDto
            {
                AppointId = s.AppointId,
                Date = s.Date,
                Timings = s.Date?.ToString("hh:mm tt")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>Books an appointment.</summary>
    public virtual async Task<bool> BookAppointmentAsync(int doctorId, int patientId, int slotId, CancellationToken ct = default)
    {
        try
        {
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Date = DateTime.Now,
                AppointmentStatus = AppointmentStatus.Pending,
                DoctorNotification = NotificationStatus.Unseen,
                PatientNotification = NotificationStatus.Unseen,
                FeedbackStatus = FeedbackStatus.Pending
            };

            await _appointmentRepo.AddAsync(appointment, ct);
            _logger.LogInformation("Appointment booked for patient {PatientId} with doctor {DoctorId}", patientId, doctorId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient {PatientId}", patientId);
            return false;
        }
    }

    /// <summary>Gets patient notification.</summary>
    public virtual async Task<AppointmentDto?> GetNotificationAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetNotificationByPatientAsync(patientId, ct);
            if (appointment == null) return null;

            return new AppointmentDto
            {
                AppointId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name,
                Timings = appointment.Date?.ToString("hh:mm tt"),
                Status = appointment.AppointmentStatus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification for patient {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets pending feedback for a patient.</summary>
    public virtual async Task<AppointmentDto?> GetPendingFeedbackAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetPendingFeedbackByPatientAsync(patientId, ct);
            if (appointment == null) return null;

            return new AppointmentDto
            {
                AppointId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name,
                Timings = appointment.Date?.ToString("hh:mm tt"),
                Status = appointment.AppointmentStatus
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Submits feedback for an appointment.</summary>
    public virtual async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct);
            if (appointment == null) return false;

            appointment.FeedbackStatus = FeedbackStatus.Given;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment {AppointmentId}", appointmentId);
            return false;
        }
    }
}
