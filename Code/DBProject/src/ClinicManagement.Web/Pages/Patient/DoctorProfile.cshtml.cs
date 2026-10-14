using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class DoctorProfileModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorDto? Doctor { get; set; }

    public DoctorProfileModel(DoctorService doctorService, ILogger<DoctorProfileModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            Doctor = await _doctorService.GetDoctorByIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor profile: {DoctorId}", doctorId);
        }

        return Page();
    }
}
