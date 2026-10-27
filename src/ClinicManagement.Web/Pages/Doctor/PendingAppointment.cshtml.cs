using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PendingAppointmentModel : PageModel
{
    private readonly DoctorService _doctorService;
    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();
    public string? Message { get; set; }

    public PendingAppointmentModel(DoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        await _doctorService.ApproveAppointmentAsync(appointmentId, ct);
        Message = "Appointment approved.";
        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        await _doctorService.DeleteAppointmentAsync(appointmentId, ct);
        Message = "Appointment rejected.";
        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, ct);
        return Page();
    }
}
