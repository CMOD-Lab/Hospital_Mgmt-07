using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for patient-related operations.
/// </summary>
public class PatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets patient information by ID.
    /// </summary>
    public async Task<PatientDto?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient with ID: {PatientId}", patientId);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient == null)
            {
                _logger.LogWarning("Patient not found with ID: {PatientId}", patientId);
                return null;
            }

            int age = CalculateAge(patient.BirthDate);

            return new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Phone = patient.Phone,
                Address = patient.Address,
                BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
                Age = age,
                Gender = patient.Gender.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets all patients, optionally filtered by search query.
    /// </summary>
    public async Task<IEnumerable<PatientListDto>> GetPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patients with search query: {SearchQuery}", searchQuery);

            IEnumerable<Patient> patients;
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                patients = await _patientRepository.GetAllAsync(cancellationToken);
            }
            else
            {
                patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);
            }

            return patients.Select(p => new PatientListDto
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Phone = p.Phone
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients");
            return Enumerable.Empty<PatientListDto>();
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
