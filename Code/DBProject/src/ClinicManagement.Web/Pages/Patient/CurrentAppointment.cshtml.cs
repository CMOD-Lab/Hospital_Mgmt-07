using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class CurrentAppointmentModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<CurrentAppointmentModel> _logger;

    public AppointmentDto? CurrentAppointment { get; set; }

    public CurrentAppointmentModel(AppointmentService appointmentService, ILogger<CurrentAppointmentModel> logger)
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
            CurrentAppointment = await _appointmentService.GetCurrentAppointmentAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current appointment for patient: {PatientId}", userId);
        }

        return Page();
    }
}
