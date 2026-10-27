using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>Login and signup page model.</summary>
public class IndexModel : PageModel
{
    private readonly AuthService _authService;
    private readonly ILogger<IndexModel> _logger;

    public string? ErrorMessage { get; set; }

    public IndexModel(AuthService authService, ILogger<IndexModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        // If already logged in, redirect to appropriate home
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId.HasValue && userType.HasValue)
        {
            return RedirectToUserHome((UserType)userType.Value);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostLoginAsync(
        string loginEmail,
        string loginPassword,
        CancellationToken ct)
    {
        var result = await _authService.ValidateLoginAsync(new LoginRequestDto
        {
            Email = loginEmail,
            Password = loginPassword
        }, ct);

        if (result.Success)
        {
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetInt32("UserType", (int)result.UserType);
            return RedirectToUserHome(result.UserType);
        }

        ErrorMessage = result.ErrorMessage;
        return Page();
    }

    public async Task<IActionResult> OnPostSignupAsync(
        string signupName,
        string signupBirthDate,
        string signupEmail,
        string signupPassword,
        string signupPhone,
        string signupGender,
        string signupAddress,
        CancellationToken ct)
    {
        var result = await _authService.RegisterPatientAsync(new PatientSignupDto
        {
            Name = signupName,
            BirthDate = signupBirthDate,
            Email = signupEmail,
            Password = signupPassword,
            PhoneNo = signupPhone,
            Gender = signupGender,
            Address = signupAddress
        }, ct);

        if (result.Success)
        {
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetInt32("UserType", (int)result.UserType);
            return RedirectToPage("/Patient/PatientHome");
        }

        ErrorMessage = result.ErrorMessage;
        return Page();
    }

    private IActionResult RedirectToUserHome(UserType userType)
    {
        return userType switch
        {
            UserType.Patient => RedirectToPage("/Patient/PatientHome"),
            UserType.Doctor => RedirectToPage("/Doctor/DoctorHome"),
            UserType.Admin => RedirectToPage("/Admin/AdminHome"),
            _ => RedirectToPage("/Index")
        };
    }
}
