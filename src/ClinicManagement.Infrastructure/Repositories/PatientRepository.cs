using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IPatientRepository.</summary>
public class PatientRepository : IPatientRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<PatientRepository> _logger;

    public PatientRepository(ClinicDbContext context, ILogger<PatientRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PatientId == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking().ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all patients");
            return Enumerable.Empty<Patient>();
        }
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string query, CancellationToken ct = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking()
                .Where(p => p.Name.Contains(query))
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with query {Query}", query);
            return Enumerable.Empty<Patient>();
        }
    }

    public async Task AddAsync(Patient patient, CancellationToken ct = default)
    {
        try
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding patient {Name}", patient.Name);
            throw;
        }
    }

    public async Task UpdateAsync(Patient patient, CancellationToken ct = default)
    {
        try
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient {Id}", patient.PatientId);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking()
                .AnyAsync(p => p.PatientId == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking patient existence {Id}", id);
            return false;
        }
    }
}
