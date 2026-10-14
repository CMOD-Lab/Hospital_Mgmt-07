using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for LoginTable entity.
/// </summary>
public interface ILoginRepository
{
    Task<LoginTable?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LoginTable?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<LoginTable?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<LoginTable> AddAsync(LoginTable login, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
