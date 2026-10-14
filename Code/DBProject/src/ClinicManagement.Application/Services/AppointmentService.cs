using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for appointment-related operations.
/// </summary>
public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets current appointment for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting current appointment for patient: {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);

            if (appointment == null) return null;

            return MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets treatment history for a patient.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting treatment history for patient: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetTreatmentHistoryAsync(patientId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting treatment history for patient: {PatientId}", patientId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets bill history for a patient.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting bill history for patient: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetBillHistoryAsync(patientId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill history for patient: {PatientId}", patientId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets free appointment slots for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting free slots for doctor: {DoctorId}", doctorId);
            var slots = await _appointmentRepository.GetFreeSlotsByDoctorAsync(doctorId, patientId, cancellationToken);
            return slots.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    public async Task<bool> CreateAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating appointment for patient: {PatientId} with doctor: {DoctorId}", dto.PatientId, dto.DoctorId);

            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                Date = DateTime.Now,
                AppointmentStatus = 2, // Pending
                DoctorNotification = 2, // Unseen
                PatientNotification = 2, // Unseen
                FeedbackStatus = 2 // Pending
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            _logger.LogInformation("Appointment created successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return false;
        }
    }

    /// <summary>
    /// Gets pending appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting pending appointments for doctor: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Gets today's appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting today's appointments for doctor: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    /// <summary>
    /// Approves an appointment.
    /// </summary>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.AppointmentStatus = 1; // Approved
            appointment.PatientNotification = 2; // Unseen
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Deletes/rejects an appointment.
    /// </summary>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting appointment: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Updates prescription for an appointment.
    /// </summary>
    public async Task<bool> UpdatePrescriptionAsync(UpdatePrescriptionDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment: {AppointmentId}", dto.AppointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.Disease = dto.Disease;
            appointment.Progress = dto.Progress;
            appointment.Prescription = dto.Prescription;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment: {AppointmentId}", dto.AppointmentId);
            return false;
        }
    }

    /// <summary>
    /// Marks appointment as paid and completed.
    /// </summary>
    public async Task<bool> MarkAsPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking appointment {AppointmentId} as paid", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.BillStatus = "Paid";
            appointment.AppointmentStatus = 3; // Completed
            appointment.FeedbackStatus = 2; // Pending
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment as paid: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Marks appointment as unpaid and completed.
    /// </summary>
    public async Task<bool> MarkAsUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking appointment {AppointmentId} as unpaid", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.BillStatus = "Unpaid";
            appointment.AppointmentStatus = 3; // Completed
            appointment.FeedbackStatus = 2; // Pending
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment as unpaid: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets pending feedback for a patient.
    /// </summary>
    public async Task<AppointmentDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting pending feedback for patient: {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetPendingFeedbackAsync(patientId, cancellationToken);
            return appointment == null ? null : MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Stores feedback for an appointment.
    /// </summary>
    public async Task<bool> StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Storing feedback for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.FeedbackStatus = 1; // Given
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing feedback for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets patient history for a doctor.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting patient history for doctor: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
            return appointments.Where(a => a.AppointmentStatus == 3).Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient history for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    private static AppointmentDto MapToDto(Appointment a)
    {
        return new AppointmentDto
        {
            AppointId = a.AppointId,
            DoctorId = a.DoctorId,
            DoctorName = a.Doctor?.Name,
            PatientId = a.PatientId,
            PatientName = a.Patient?.Name,
            Date = a.Date,
            Timings = a.Date?.ToString("hh:mm tt"),
            AppointmentStatus = a.AppointmentStatus,
            BillAmount = a.BillAmount,
            BillStatus = a.BillStatus,
            Disease = a.Disease,
            Progress = a.Progress,
            Prescription = a.Prescription,
            FeedbackStatus = a.FeedbackStatus
        };
    }
}
