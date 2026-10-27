using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Repository interface for Appointment data access.</summary>
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetPendingByDoctorAsync(int doctorId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetTodaysByDoctorAsync(int doctorId, CancellationToken ct = default);
    Task<Appointment?> GetCurrentByPatientAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetBillHistoryByPatientAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetTreatmentHistoryByPatientAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken ct = default);
    Task<Appointment?> GetPendingFeedbackByPatientAsync(int patientId, CancellationToken ct = default);
    Task<Appointment?> GetNotificationByPatientAsync(int patientId, CancellationToken ct = default);
    Task<IEnumerable<Appointment>> GetBillableByDoctorAsync(int doctorId, CancellationToken ct = default);
    Task AddAsync(Appointment appointment, CancellationToken ct = default);
    Task UpdateAsync(Appointment appointment, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
