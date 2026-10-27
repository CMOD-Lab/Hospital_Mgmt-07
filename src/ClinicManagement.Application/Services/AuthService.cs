using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Service for authentication operations.</summary>
public class AuthService
{
    private readonly ILoginRepository _loginRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ILoginRepository loginRepo, IPatientRepository patientRepo, ILogger<AuthService> logger)
    {
        _loginRepo = loginRepo;
        _patientRepo = patientRepo;
        _logger = logger;
    }

    /// <summary>Validates login credentials and returns result.</summary>
    public virtual async Task<LoginResultDto> ValidateLoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var login = await _loginRepo.ValidateAsync(request.Email, request.Password, ct);
            if (login == null)
            {
                // Check if email exists
                var emailExists = await _loginRepo.EmailExistsAsync(request.Email, ct);
                if (!emailExists)
                    return new LoginResultDto { Success = false, ErrorMessage = "Email not found. Try Again!" };
                return new LoginResultDto { Success = false, ErrorMessage = "Incorrect Password. Try Again!" };
            }

            _logger.LogInformation("User {Email} logged in successfully as {Type}", request.Email, login.Type);
            return new LoginResultDto { Success = true, UserId = login.LoginId, UserType = login.Type };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return new LoginResultDto { Success = false, ErrorMessage = "There was some error. Try Again!" };
        }
    }

    /// <summary>Registers a new patient.</summary>
    public virtual async Task<LoginResultDto> RegisterPatientAsync(PatientSignupDto dto, CancellationToken ct = default)
    {
        try
        {
            var emailExists = await _loginRepo.EmailExistsAsync(dto.Email, ct);
            if (emailExists)
                return new LoginResultDto { Success = false, ErrorMessage = "Email already exists. Please choose a different one." };

            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = UserType.Patient
            };

            var loginId = await _loginRepo.AddAsync(login, ct);

            if (!DateOnly.TryParse(dto.BirthDate, out var birthDate))
                return new LoginResultDto { Success = false, ErrorMessage = "Invalid birth date format." };

            var patient = new Patient
            {
                PatientId = loginId,
                Name = dto.Name,
                Phone = dto.PhoneNo,
                Address = dto.Address,
                BirthDate = birthDate,
                Gender = string.IsNullOrEmpty(dto.Gender) ? 'M' : dto.Gender[0]
            };

            await _patientRepo.AddAsync(patient, ct);

            _logger.LogInformation("New patient registered: {Email}", dto.Email);
            return new LoginResultDto { Success = true, UserId = loginId, UserType = UserType.Patient };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration for {Email}", dto.Email);
            return new LoginResultDto { Success = false, ErrorMessage = "There was some error. Try again!" };
        }
    }
}
