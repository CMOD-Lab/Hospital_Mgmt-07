namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for login validation result.
/// </summary>
public class LoginResultDto
{
    public bool Success { get; set; }
    public int UserId { get; set; }
    public int UserType { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// DTO for login request.
/// </summary>
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO for patient signup.
/// </summary>
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

/// <summary>
/// DTO for patient data display.
/// </summary>
public class PatientDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? BirthDate { get; set; }
    public int Age { get; set; }
    public string? Gender { get; set; }
}

/// <summary>
/// DTO for patient list view.
/// </summary>
public class PatientListDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}
