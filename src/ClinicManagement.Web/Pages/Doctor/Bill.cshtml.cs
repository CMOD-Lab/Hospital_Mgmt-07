using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly DoctorService _doctorService;
    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();
    public string? Message { get; set; }

    public BillModel(DoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        Appointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostPaidAsync(int appointmentId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        await _doctorService.MarkAsPaidAsync(userId.Value, appointmentId, ct);
        Message = "Appointment marked as paid.";
        Appointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostUnpaidAsync(int appointmentId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        await _doctorService.MarkAsUnpaidAsync(userId.Value, appointmentId, ct);
        Message = "Appointment marked as unpaid.";
        Appointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value, ct);
        return Page();
    }
}
