using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientNotificationsModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PatientNotificationsModel> _logger;

    public AppointmentDto? Notification { get; set; }

    public PatientNotificationsModel(AppointmentService appointmentService, ILogger<PatientNotificationsModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            // Get the most recent approved appointment as notification
            var appointments = await _appointmentService.GetBillHistoryAsync(userId.Value, cancellationToken);
            Notification = appointments
                .Where(a => a.AppointmentStatus == 1)
                .OrderByDescending(a => a.Date)
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications for patient: {PatientId}", userId);
        }

        return Page();
    }
}
