using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IDoctorRepository.</summary>
public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DoctorRepository> _logger;

    public DoctorRepository(ClinicDbContext context, ILogger<DoctorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.Doctors.AsNoTracking()
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DoctorId == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctor {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Doctor>> GetAllActiveAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.Doctors.AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == 1)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all active doctors");
            return Enumerable.Empty<Doctor>();
        }
    }

    public async Task<IEnumerable<Doctor>> SearchAsync(string query, CancellationToken ct = default)
    {
        try
        {
            return await _context.Doctors.AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == 1 && d.Name.Contains(query))
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors with query {Query}", query);
            return Enumerable.Empty<Doctor>();
        }
    }

    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(string deptName, CancellationToken ct = default)
    {
        try
        {
            return await _context.Doctors.AsNoTracking()
                .Include(d => d.Department)
                .Where(d => d.Status == 1 && d.Department != null && d.Department.DeptName == deptName)
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors for department {DeptName}", deptName);
            return Enumerable.Empty<Doctor>();
        }
    }

    public async Task AddAsync(Doctor doctor, CancellationToken ct = default)
    {
        try
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor {Name}", doctor.Name);
            throw;
        }
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken ct = default)
    {
        try
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor {Id}", doctor.DoctorId);
            throw;
        }
    }

    public async Task SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var doctor = await _context.Doctors.FindAsync(new object[] { id }, ct);
            if (doctor != null)
            {
                doctor.Status = 0;
                await _context.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft-deleting doctor {Id}", id);
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
            _logger.LogError(ex, "Error checking doctor email {Email}", email);
            return false;
        }
    }
}
