using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IAppointmentRepository.
/// </summary>
public class AppointmentRepository : IAppointmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(ClinicDbContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all appointments");
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments for patient: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.AppointmentStatus == 2) // Pending
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId
                    && a.AppointmentStatus == 1 // Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.Today;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId
                    && a.AppointmentStatus == 1 // Approved
                    && a.Date.HasValue
                    && a.Date.Value.Date == today)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.AppointmentStatus == 3) // Completed
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting treatment history for patient: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.AppointmentStatus == 3) // Completed
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bill history for patient: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId && a.FeedbackStatus == 2) // Pending
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Return appointments that are available (not yet taken by this patient)
            var today = DateTime.Today;
            return await _context.Appointments.AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId
                    && a.PatientId == null
                    && a.Date.HasValue
                    && a.Date.Value.Date >= today)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(cancellationToken);
            return appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding appointment");
            throw;
        }
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment with ID: {Id}", appointment.AppointId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Appointments.AsNoTracking()
                .AnyAsync(a => a.AppointId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking appointment existence for ID: {Id}", id);
            return false;
        }
    }
}
