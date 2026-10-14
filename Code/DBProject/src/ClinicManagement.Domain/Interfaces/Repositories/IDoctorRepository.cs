using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Doctor entity.
/// </summary>
public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetByDepartmentAsync(string deptName, CancellationToken cancellationToken = default);
    Task<Doctor> AddAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
