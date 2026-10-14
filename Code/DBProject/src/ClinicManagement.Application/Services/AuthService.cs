using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Exceptions;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for authentication and user management.
/// </summary>
public class AuthService
{
    private readonly ILoginRepository _loginRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILoginRepository loginRepository,
        IPatientRepository patientRepository,
        ILogger<AuthService> logger)
    {
        _loginRepository = loginRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Validates login credentials and returns login result.
    /// </summary>
    public async Task<LoginResultDto> ValidateLoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var login = await _loginRepository.ValidateLoginAsync(request.Email, request.Password, cancellationToken);

            if (login == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                return new LoginResultDto { Success = false, Message = "Invalid email or password." };
            }

            _logger.LogInformation("Login successful for user ID: {UserId}", login.LoginId);
            return new LoginResultDto
            {
                Success = true,
                UserId = login.LoginId,
                UserType = login.Type,
                Message = "Login successful."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return new LoginResultDto { Success = false, Message = "An error occurred. Please try again." };
        }
    }

    /// <summary>
    /// Registers a new patient.
    /// </summary>
    public async Task<LoginResultDto> RegisterPatientAsync(PatientSignupDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Patient registration attempt for email: {Email}", dto.Email);

            // Check if email already exists
            bool emailExists = await _loginRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists)
            {
                return new LoginResultDto { Success = false, Message = "Email already exists. Please choose a different one." };
            }

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 1 // Patient
            };

            var createdLogin = await _loginRepository.AddAsync(login, cancellationToken);

            // Create patient entry
            var patient = new Patient
            {
                PatientId = createdLogin.LoginId,
                Name = dto.Name,
                Phone = dto.PhoneNo,
                Address = dto.Address,
                BirthDate = DateTime.Parse(dto.BirthDate),
                Gender = string.IsNullOrEmpty(dto.Gender) ? 'M' : dto.Gender[0]
            };

            await _patientRepository.AddAsync(patient, cancellationToken);

            _logger.LogInformation("Patient registered successfully with ID: {PatientId}", createdLogin.LoginId);
            return new LoginResultDto
            {
                Success = true,
                UserId = createdLogin.LoginId,
                UserType = 1,
                Message = "Registration successful."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration for email: {Email}", dto.Email);
            return new LoginResultDto { Success = false, Message = "An error occurred during registration. Please try again." };
        }
    }
}
