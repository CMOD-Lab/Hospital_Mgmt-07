using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Repository interface for LoginTable data access.</summary>
public interface ILoginRepository
{
    Task<LoginTable?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<LoginTable?> ValidateAsync(string email, string password, CancellationToken ct = default);
    Task<LoginTable?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> AddAsync(LoginTable login, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}
