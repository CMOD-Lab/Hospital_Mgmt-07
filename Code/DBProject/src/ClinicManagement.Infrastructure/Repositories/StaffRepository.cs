using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of IStaffRepository.
/// </summary>
public class StaffRepository : IStaffRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(ClinicDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OtherStaff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff by ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all staff");
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task<IEnumerable<OtherStaff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .Where(s => s.Name.Contains(searchQuery))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query: {Query}", searchQuery);
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.OtherStaff.Add(staff);
            await _context.SaveChangesAsync(cancellationToken);
            return staff;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff");
            throw;
        }
    }

    public async Task UpdateAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.OtherStaff.Update(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff with ID: {Id}", staff.StaffId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _context.OtherStaff.FindAsync(new object[] { id }, cancellationToken);
            if (staff != null)
            {
                _context.OtherStaff.Remove(staff);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff with ID: {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .AnyAsync(s => s.StaffId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking staff existence for ID: {Id}", id);
            return false;
        }
    }
}
