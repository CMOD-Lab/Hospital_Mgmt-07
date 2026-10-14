using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PendingAppointmentModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public IEnumerable<AppointmentDto> PendingAppointments { get; set; } = new List<AppointmentDto>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public PendingAppointmentModel(AppointmentService appointmentService, ILogger<PendingAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            PendingAppointments = await _appointmentService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments for doctor: {DoctorId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            bool result = await _appointmentService.ApproveAppointmentAsync(appointmentId, cancellationToken);
            Success = result;
            Message = result ? "Appointment approved successfully." : "Error approving appointment.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
            Message = "An error occurred.";
        }

        PendingAppointments = await _appointmentService.GetPendingAppointmentsAsync(userId!.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            bool result = await _appointmentService.DeleteAppointmentAsync(appointmentId, cancellationToken);
            Success = result;
            Message = result ? "Appointment rejected." : "Error rejecting appointment.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment: {AppointmentId}", appointmentId);
            Message = "An error occurred.";
        }

        PendingAppointments = await _appointmentService.GetPendingAppointmentsAsync(userId!.Value, cancellationToken);
        return Page();
    }
}
