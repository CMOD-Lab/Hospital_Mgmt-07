using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly PatientService _patientService;
    public AppointmentDto? PendingFeedback { get; set; }
    public string? Message { get; set; }

    public PatientFeedbackModel(PatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");

        await _patientService.SubmitFeedbackAsync(appointmentId, ct);
        Message = "Thank you for your feedback!";
        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, ct);
        return Page();
    }
}
