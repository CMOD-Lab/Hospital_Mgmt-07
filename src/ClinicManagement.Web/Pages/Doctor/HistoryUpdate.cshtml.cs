using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HistoryUpdateModel : PageModel
{
    private readonly DoctorService _doctorService;
    public AppointmentDto? CurrentAppointment { get; set; }
    public int AppointmentId { get; set; }
    public string? Message { get; set; }

    public HistoryUpdateModel(DoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(int appointmentId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        AppointmentId = appointmentId;
        var appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value, ct);
        CurrentAppointment = appointments.FirstOrDefault(a => a.AppointId == appointmentId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, string disease, string progress, string prescription, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");

        await _doctorService.UpdatePrescriptionAsync(new UpdatePrescriptionDto
        {
            DoctorId = userId.Value,
            AppointmentId = appointmentId,
            Disease = disease,
            Progress = progress,
            Prescription = prescription
        }, ct);

        Message = "Prescription updated successfully!";
        AppointmentId = appointmentId;
        var appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value, ct);
        CurrentAppointment = appointments.FirstOrDefault(a => a.AppointId == appointmentId);
        return Page();
    }
}
