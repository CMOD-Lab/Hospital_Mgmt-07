using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly PatientService _patientService;
    public IEnumerable<AppointmentDto> History { get; set; } = Enumerable.Empty<AppointmentDto>();

    public TreatmentHistoryModel(PatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        History = await _patientService.GetTreatmentHistoryAsync(userId.Value, ct);
        return Page();
    }
}
