using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HistoryUpdateModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    public AppointmentDto? Appointment { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public HistoryUpdateModel(AppointmentService appointmentService, ILogger<HistoryUpdateModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            var appointments = await _appointmentService.GetTodaysAppointmentsAsync(userId.Value, cancellationToken);
            Appointment = appointments.FirstOrDefault(a => a.AppointId == appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment: {AppointmentId}", appointmentId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        int appointmentId, string disease, string progress, string prescription,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            var dto = new UpdatePrescriptionDto
            {
                AppointmentId = appointmentId,
                DoctorId = userId.Value,
                Disease = disease,
                Progress = progress,
                Prescription = prescription
            };

            bool result = await _appointmentService.UpdatePrescriptionAsync(dto, cancellationToken);
            Success = result;
            Message = result ? "History updated successfully!" : "Error updating history.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating history for appointment: {AppointmentId}", appointmentId);
            Message = "An error occurred.";
        }

        var appointments = await _appointmentService.GetTodaysAppointmentsAsync(userId!.Value, cancellationToken);
        Appointment = appointments.FirstOrDefault(a => a.AppointId == appointmentId);
        return Page();
    }
}
