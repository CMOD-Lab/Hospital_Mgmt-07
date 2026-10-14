using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IDepartmentRepository.
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(ClinicDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync(d => d.DeptNo == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .Include(d => d.Doctors)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments");
            return Enumerable.Empty<Department>();
        }
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync(d => d.DeptName == name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department by name: {Name}", name);
            return null;
        }
    }

    public async Task<Department> AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);
            return department;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding department");
            throw;
        }
    }

    public async Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating department with ID: {Id}", department.DeptNo);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var dept = await _context.Departments.FindAsync(new object[] { id }, cancellationToken);
            if (dept != null)
            {
                _context.Departments.Remove(dept);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .AnyAsync(d => d.DeptNo == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking department existence for ID: {Id}", id);
            return false;
        }
    }
}
