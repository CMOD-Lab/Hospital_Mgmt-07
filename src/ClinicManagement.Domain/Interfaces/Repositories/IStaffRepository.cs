using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Repository interface for OtherStaff data access.</summary>
public interface IStaffRepository
{
    Task<OtherStaff?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<OtherStaff>> SearchAsync(string query, CancellationToken ct = default);
    Task AddAsync(OtherStaff staff, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
