using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for login result.</summary>
public class LoginResultDto
{
    public bool Success { get; set; }
    public int UserId { get; set; }
    public UserType UserType { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>DTO for login request.</summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>DTO for patient signup.</summary>
public class PatientSignupDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
