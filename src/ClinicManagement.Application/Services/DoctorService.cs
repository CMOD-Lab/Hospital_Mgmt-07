using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service for doctor-facing operations.</summary>
public class DoctorService
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepo,
        IAppointmentRepository appointmentRepo,
        ILogger<DoctorService> logger)
    {
        _doctorRepo = doctorRepo;
        _appointmentRepo = appointmentRepo;
        _logger = logger;
    }

    /// <summary>Gets doctor profile information.</summary>
    public async Task<DoctorProfileDto?> GetDoctorInfoAsync(int doctorId, CancellationToken ct = default)
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
            _logger.LogError(ex, "Error getting doctor info for {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>Gets pending appointments for a doctor.</summary>
    public async Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetPendingByDoctorAsync(doctorId, ct);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                Timings = a.Date?.ToString("hh:mm tt"),
                Status = a.AppointmentStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>Gets today's appointments for a doctor.</summary>
    public async Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetTodaysByDoctorAsync(doctorId, ct);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                Timings = a.Date?.ToString("hh:mm tt"),
                Status = a.AppointmentStatus,
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>Approves an appointment.</summary>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct);
            if (appointment == null) return false;

            appointment.AppointmentStatus = AppointmentStatus.Approved;
            appointment.PatientNotification = NotificationStatus.Unseen;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            _logger.LogInformation("Appointment {AppointmentId} approved", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>Rejects/deletes an appointment.</summary>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken ct = default)
    {
        try
        {
            await _appointmentRepo.DeleteAsync(appointmentId, ct);
            _logger.LogInformation("Appointment {AppointmentId} deleted", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>Updates prescription for an appointment.</summary>
    public async Task<bool> UpdatePrescriptionAsync(UpdatePrescriptionDto dto, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(dto.AppointmentId, ct);
            if (appointment == null) return false;

            appointment.Disease = dto.Disease;
            appointment.Progress = dto.Progress;
            appointment.Prescription = dto.Prescription;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            _logger.LogInformation("Prescription updated for appointment {AppointmentId}", dto.AppointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment {AppointmentId}", dto.AppointmentId);
            return false;
        }
    }

    /// <summary>Gets billable appointments for a doctor.</summary>
    public async Task<IEnumerable<AppointmentDto>> GetBillableAppointmentsAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetBillableByDoctorAsync(doctorId, ct);
            return appointments.Select(a => new AppointmentDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus?.ToString(),
                Status = a.AppointmentStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting billable appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>Marks appointment as paid and completed.</summary>
    public async Task<bool> MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct);
            if (appointment == null) return false;

            appointment.BillStatus = BillStatus.Paid;
            appointment.AppointmentStatus = AppointmentStatus.Completed;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment {AppointmentId} as paid", appointmentId);
            return false;
        }
    }

    /// <summary>Marks appointment as unpaid and completed.</summary>
    public async Task<bool> MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct);
            if (appointment == null) return false;

            appointment.BillStatus = BillStatus.Unpaid;
            appointment.AppointmentStatus = AppointmentStatus.Completed;
            await _appointmentRepo.UpdateAsync(appointment, ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment {AppointmentId} as unpaid", appointmentId);
            return false;
        }
    }

    /// <summary>Gets patient history for a doctor.</summary>
    public async Task<IEnumerable<AppointmentDto>> GetPatientHistoryAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var appointments = await _appointmentRepo.GetByDoctorIdAsync(doctorId, ct);
            return appointments
                .Where(a => a.AppointmentStatus == AppointmentStatus.Completed)
                .Select(a => new AppointmentDto
                {
                    AppointId = a.AppointId,
                    PatientName = a.Patient?.Name,
                    Date = a.Date,
                    Disease = a.Disease,
                    Progress = a.Progress,
                    Prescription = a.Prescription,
                    BillAmount = a.BillAmount,
                    BillStatus = a.BillStatus?.ToString(),
                    Status = a.AppointmentStatus
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient history for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }
}
