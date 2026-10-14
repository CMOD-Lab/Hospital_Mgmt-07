using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for OtherStaff entity.
/// </summary>
public interface IStaffRepository
{
    Task<OtherStaff?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherStaff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken cancellationToken = default);
    Task UpdateAsync(OtherStaff staff, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
