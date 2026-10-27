using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IDepartmentRepository.</summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(ClinicDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptNo == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking().ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments");
            return Enumerable.Empty<Department>();
        }
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptName == name, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting department by name {Name}", name);
            return null;
        }
    }
}
