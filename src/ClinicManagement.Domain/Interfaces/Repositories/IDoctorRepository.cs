using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Repository interface for Doctor data access.</summary>
public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Doctor>> GetAllActiveAsync(CancellationToken ct = default);
    Task<IEnumerable<Doctor>> SearchAsync(string query, CancellationToken ct = default);
    Task<IEnumerable<Doctor>> GetByDepartmentAsync(string deptName, CancellationToken ct = default);
    Task AddAsync(Doctor doctor, CancellationToken ct = default);
    Task UpdateAsync(Doctor doctor, CancellationToken ct = default);
    Task SoftDeleteAsync(int id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}
