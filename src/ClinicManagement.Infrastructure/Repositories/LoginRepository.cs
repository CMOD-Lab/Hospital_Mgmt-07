using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of ILoginRepository.</summary>
public class LoginRepository : ILoginRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<LoginRepository> _logger;

    public LoginRepository(ClinicDbContext context, ILogger<LoginRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LoginTable?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        try
        {
            return await _context.LoginTable.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting login by email {Email}", email);
            return null;
        }
    }

    public async Task<LoginTable?> ValidateAsync(string email, string password, CancellationToken ct = default)
    {
        try
        {
            return await _context.LoginTable.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email && l.Password == password, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for {Email}", email);
            return null;
        }
    }

    public async Task<LoginTable?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.LoginTable.AsNoTracking()
                .FirstOrDefaultAsync(l => l.LoginId == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting login by id {Id}", id);
            return null;
        }
    }

    public async Task<int> AddAsync(LoginTable login, CancellationToken ct = default)
    {
        try
        {
            _context.LoginTable.Add(login);
            await _context.SaveChangesAsync(ct);
            return login.LoginId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding login for {Email}", login.Email);
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        try
        {
            return await _context.LoginTable.AsNoTracking()
                .AnyAsync(l => l.Email == email, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence {Email}", email);
            return false;
        }
    }
}
