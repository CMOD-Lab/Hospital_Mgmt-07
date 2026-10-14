using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for staff-related operations.
/// </summary>
public class StaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<StaffService> _logger;

    public StaffService(IStaffRepository staffRepository, ILogger<StaffService> logger)
    {
        _staffRepository = staffRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets staff member by ID.
    /// </summary>
    public async Task<StaffDto?> GetStaffByIdAsync(int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving staff with ID: {StaffId}", staffId);
            var staff = await _staffRepository.GetByIdAsync(staffId, cancellationToken);

            if (staff == null)
            {
                _logger.LogWarning("Staff not found with ID: {StaffId}", staffId);
                return null;
            }

            return new StaffDto
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Phone = staff.Phone,
                Address = staff.Address,
                Gender = staff.Gender.ToString(),
                Designation = staff.Designation,
                Salary = staff.Salary
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID: {StaffId}", staffId);
            return null;
        }
    }

    /// <summary>
    /// Gets all staff, optionally filtered by search query.
    /// </summary>
    public async Task<IEnumerable<StaffListDto>> GetStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving staff with search query: {SearchQuery}", searchQuery);

            IEnumerable<OtherStaff> staffList;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                staffList = await _staffRepository.GetAllAsync(cancellationToken);
            }
            else
            {
                staffList = await _staffRepository.SearchAsync(searchQuery, cancellationToken);
            }

            return staffList.Select(s => new StaffListDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Designation = s.Designation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff");
            return Enumerable.Empty<StaffListDto>();
        }
    }

    /// <summary>
    /// Adds a new staff member.
    /// </summary>
    public async Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new staff member: {Name}", dto.Name);

            var staff = new OtherStaff
            {
                Name = dto.Name,
                BirthDate = string.IsNullOrEmpty(dto.BirthDate) ? null : DateTime.Parse(dto.BirthDate),
                Phone = dto.Phone,
                Gender = dto.Gender,
                Salary = dto.Salary,
                Designation = dto.Designation,
                HighestQualification = dto.Qualification,
                Address = dto.Address
            };

            await _staffRepository.AddAsync(staff, cancellationToken);
            _logger.LogInformation("Staff member added successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>
    /// Deletes a staff member.
    /// </summary>
    public async Task<bool> DeleteStaffAsync(int staffId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff with ID: {StaffId}", staffId);
            await _staffRepository.DeleteAsync(staffId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff with ID: {StaffId}", staffId);
            return false;
        }
    }
}
