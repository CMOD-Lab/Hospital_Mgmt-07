using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class AppointmentTakerModel : PageModel
{
    private readonly PatientService _patientService;
    public IEnumerable<AppointmentDto> FreeSlots { get; set; } = Enumerable.Empty<AppointmentDto>();
    public int DoctorId { get; set; }
    public string? Message { get; set; }

    public AppointmentTakerModel(PatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        DoctorId = doctorId;
        FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int slotId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");

        var success = await _patientService.BookAppointmentAsync(doctorId, userId.Value, slotId, ct);
        if (success)
            return RedirectToPage("/Patient/AppointmentRequestSent");

        Message = "Error booking appointment. Please try again.";
        DoctorId = doctorId;
        FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value, ct);
        return Page();
    }
}
