using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>
/// Page model for the login and signup page.
/// </summary>
public class SignUpModel : PageModel
{
    private readonly AuthService _authService;
    private readonly ILogger<SignUpModel> _logger;

    public string LoginMessage { get; set; } = string.Empty;
    public bool LoginSuccess { get; set; }
    public string SignupMessage { get; set; } = string.Empty;
    public bool SignupSuccess { get; set; }

    public SignUpModel(AuthService authService, ILogger<SignUpModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        // Clear session on page load
        HttpContext.Session.Clear();
        return Page();
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    public async Task<IActionResult> OnPostLoginAsync(
        string loginEmail,
        string loginPassword,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new LoginRequestDto
            {
                Email = loginEmail,
                Password = loginPassword
            };

            var result = await _authService.ValidateLoginAsync(request, cancellationToken);

            if (result.Success)
            {
                HttpContext.Session.SetInt32("UserId", result.UserId);
                HttpContext.Session.SetInt32("UserType", result.UserType);

                return result.UserType switch
                {
                    1 => RedirectToPage("/Patient/PatientHome"),
                    2 => RedirectToPage("/Doctor/DoctorHome"),
                    3 => RedirectToPage("/Admin/AdminHome"),
                    _ => Page()
                };
            }

            LoginMessage = result.Message;
            LoginSuccess = false;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            LoginMessage = "An error occurred. Please try again.";
            return Page();
        }
    }

    /// <summary>
    /// Handles patient signup form submission.
    /// </summary>
    public async Task<IActionResult> OnPostSignupAsync(
        string sName,
        string sBirthDate,
        string sEmail,
        string sPassword,
        string Phone,
        string Address,
        string Gender,
        CancellationToken cancellationToken)
    {
        try
        {
            var dto = new PatientSignupDto
            {
                Name = sName,
                BirthDate = sBirthDate,
                Email = sEmail,
                Password = sPassword,
                PhoneNo = Phone,
                Address = Address,
                Gender = Gender
            };

            var result = await _authService.RegisterPatientAsync(dto, cancellationToken);

            if (result.Success)
            {
                HttpContext.Session.SetInt32("UserId", result.UserId);
                HttpContext.Session.SetInt32("UserType", result.UserType);
                return RedirectToPage("/Patient/PatientHome");
            }

            SignupMessage = result.Message;
            SignupSuccess = false;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient signup");
            SignupMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }

    /// <summary>
    /// Handles logout.
    /// </summary>
    public IActionResult OnGetLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/SignUp");
    }
}
