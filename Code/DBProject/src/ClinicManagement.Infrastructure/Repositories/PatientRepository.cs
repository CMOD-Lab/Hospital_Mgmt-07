using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IPatientRepository.
/// </summary>
public class PatientRepository : IPatientRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<PatientRepository> _logger;

    public PatientRepository(ClinicDbContext context, ILogger<PatientRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking()
                .FirstOrDefaultAsync(p => p.PatientId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting patient by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all patients");
            return Enumerable.Empty<Patient>();
        }
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking()
                .Where(p => p.Name.Contains(searchQuery))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with query: {Query}", searchQuery);
            return Enumerable.Empty<Patient>();
        }
    }

    public async Task<Patient> AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);
            return patient;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding patient");
            throw;
        }
    }

    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient with ID: {Id}", patient.PatientId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _context.Patients.FindAsync(new object[] { id }, cancellationToken);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting patient with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Patients.AsNoTracking()
                .AnyAsync(p => p.PatientId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking patient existence for ID: {Id}", id);
            return false;
        }
    }
}
