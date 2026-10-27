using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class DoctorProfileModel : PageModel
{
    private readonly PatientService _patientService;
    public DoctorProfileDto? Doctor { get; set; }

    public DoctorProfileModel(PatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        Doctor = await _patientService.GetDoctorProfileAsync(doctorId, ct);
        return Page();
    }
}
