using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>EF Core implementation of IStaffRepository.</summary>
public class StaffRepository : IStaffRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(ClinicDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OtherStaff?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking().ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all staff");
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task<IEnumerable<OtherStaff>> SearchAsync(string query, CancellationToken ct = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .Where(s => s.Name.Contains(query))
                .ToListAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query {Query}", query);
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task AddAsync(OtherStaff staff, CancellationToken ct = default)
    {
        try
        {
            _context.OtherStaff.Add(staff);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff {Name}", staff.Name);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var staff = await _context.OtherStaff.FindAsync(new object[] { id }, ct);
            if (staff != null)
            {
                _context.OtherStaff.Remove(staff);
                await _context.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff {Id}", id);
            throw;
        }
    }
}
