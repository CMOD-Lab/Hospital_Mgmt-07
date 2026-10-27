using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PreviousHistoryModel : PageModel
{
    private readonly DoctorService _doctorService;
    public IEnumerable<AppointmentDto> History { get; set; } = Enumerable.Empty<AppointmentDto>();

    public PreviousHistoryModel(DoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        History = await _doctorService.GetPatientHistoryAsync(userId.Value, ct);
        return Page();
    }
}
