using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient home page model.</summary>
public class PatientHomeModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientDto? PatientInfo { get; set; }

    public PatientHomeModel(PatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Index");

        PatientInfo = await _patientService.GetPatientInfoAsync(userId.Value, ct);
        return Page();
    }
}
