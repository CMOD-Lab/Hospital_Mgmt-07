using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for doctor-related operations.
/// </summary>
public class DoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IDepartmentRepository departmentRepository,
        ILoginRepository loginRepository,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _loginRepository = loginRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets doctor profile by ID.
    /// </summary>
    public async Task<DoctorDto?> GetDoctorByIdAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor with ID: {DoctorId}", doctorId);
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);

            if (doctor == null)
            {
                _logger.LogWarning("Doctor not found with ID: {DoctorId}", doctorId);
                return null;
            }

            int age = CalculateAge(doctor.BirthDate);

            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender.ToString(),
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex ?? 0,
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience,
                Age = age,
                Department = doctor.Department?.DeptName
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>
    /// Gets all active doctors, optionally filtered by search query.
    /// </summary>
    public async Task<IEnumerable<DoctorListDto>> GetDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctors with search query: {SearchQuery}", searchQuery);

            IEnumerable<Doctor> doctors;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                doctors = await _doctorRepository.GetActiveAsync(cancellationToken);
            }
            else
            {
                doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
            }

            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Department = d.Department?.DeptName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors");
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>
    /// Gets doctors by department name.
    /// </summary>
    public async Task<IEnumerable<DoctorListDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctors for department: {DeptName}", deptName);
            var doctors = await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);

            return doctors.Select(d => new DoctorListDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Department = d.Department?.DeptName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department: {DeptName}", deptName);
            return Enumerable.Empty<DoctorListDto>();
        }
    }

    /// <summary>
    /// Adds a new doctor to the system.
    /// </summary>
    public async Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new doctor with email: {Email}", dto.Email);

            // Check if email already exists
            bool emailExists = await _loginRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Doctor email already exists: {Email}", dto.Email);
                return false;
            }

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 2 // Doctor
            };

            var createdLogin = await _loginRepository.AddAsync(login, cancellationToken);

            // Create doctor entry
            var doctor = new Doctor
            {
                DoctorId = createdLogin.LoginId,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address,
                BirthDate = DateTime.Parse(dto.BirthDate),
                DeptNo = dto.DeptNo,
                Gender = dto.Gender,
                WorkExperience = dto.WorkExperience,
                MonthlySalary = dto.Salary,
                ChargesPerVisit = dto.ChargesPerVisit,
                Specialization = dto.Specialization,
                Qualification = dto.Qualification,
                Status = 1 // Present
            };

            await _doctorRepository.AddAsync(doctor, cancellationToken);

            _logger.LogInformation("Doctor added successfully with ID: {DoctorId}", createdLogin.LoginId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor with email: {Email}", dto.Email);
            return false;
        }
    }

    /// <summary>
    /// Soft-deletes a doctor (sets status to 0).
    /// </summary>
    public async Task<bool> DeleteDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Soft-deleting doctor with ID: {DoctorId}", doctorId);
            return await _doctorRepository.SoftDeleteAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID: {DoctorId}", doctorId);
            return false;
        }
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        int age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}
