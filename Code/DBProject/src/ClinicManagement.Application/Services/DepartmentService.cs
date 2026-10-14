using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for department-related operations.
/// </summary>
public class DepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets all departments.
    /// </summary>
    public async Task<IEnumerable<DepartmentViewDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all departments");
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);

            return departments.Select(d => new DepartmentViewDto
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName,
                Description = d.Description,
                DoctorCount = d.Doctors?.Count(doc => doc.Status == 1) ?? 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return Enumerable.Empty<DepartmentViewDto>();
        }
    }
}
