using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly PatientService _patientService;
    public IEnumerable<AppointmentDto> Bills { get; set; } = Enumerable.Empty<AppointmentDto>();

    public BillsHistoryModel(PatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        Bills = await _patientService.GetBillHistoryAsync(userId.Value, ct);
        return Page();
    }
}
