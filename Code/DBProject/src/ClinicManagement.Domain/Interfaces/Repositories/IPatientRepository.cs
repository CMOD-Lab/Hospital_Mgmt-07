using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Patient entity.
/// </summary>
public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<Patient> AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
