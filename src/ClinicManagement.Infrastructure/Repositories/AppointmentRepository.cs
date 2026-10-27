using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IAppointmentRepository.</summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(ClinicDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointId == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments for patient {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetPendingByDoctorAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentStatus == AppointmentStatus.Pending)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentStatus == AppointmentStatus.Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetCurrentByPatientAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.AppointmentStatus == AppointmentStatus.Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetBillHistoryByPatientAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.AppointmentStatus == AppointmentStatus.Completed)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill history for patient {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTreatmentHistoryByPatientAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.AppointmentStatus == AppointmentStatus.Completed)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting treatment history for patient {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken ct = default)
    {
        try
        {
            // Return future approved appointments that are not yet taken by this patient
            var tomorrow = DateTime.Today.AddDays(1);
            return await _context.Appointments.AsNoTracking()
                .Where(a => a.DoctorId == doctorId
                    && a.PatientId == null
                    && a.Date.HasValue
                    && a.Date.Value >= tomorrow)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetPendingFeedbackByPatientAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.FeedbackStatus == FeedbackStatus.Pending
                    && a.AppointmentStatus == AppointmentStatus.Completed, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient {PatientId}", patientId);
            return null;
        }
    }

    public async Task<Appointment?> GetNotificationByPatientAsync(int patientId, CancellationToken ct = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.PatientId == patientId
                    && a.PatientNotification == NotificationStatus.Unseen, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notification for patient {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetBillableByDoctorAsync(int doctorId, CancellationToken ct = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentStatus == AppointmentStatus.Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting billable appointments for doctor {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task AddAsync(Appointment appointment, CancellationToken ct = default)
    {
        try
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding appointment");
            throw;
        }
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken ct = default)
    {
        try
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {Id}", appointment.AppointId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { id }, ct);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {Id}", id);
            throw;
        }
    }
}
