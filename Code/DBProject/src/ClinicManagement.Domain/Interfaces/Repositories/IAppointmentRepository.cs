using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Appointment entity.
/// </summary>
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetPendingByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetTodaysByDoctorIdAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetCurrentByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
