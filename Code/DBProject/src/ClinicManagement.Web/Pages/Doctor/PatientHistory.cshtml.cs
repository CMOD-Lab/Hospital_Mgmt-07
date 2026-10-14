using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PatientHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public IEnumerable<AppointmentDto> TodaysAppointments { get; set; } = new List<AppointmentDto>();

    public PatientHistoryModel(AppointmentService appointmentService, ILogger<PatientHistoryModel> logger)
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
            TodaysAppointments = await _appointmentService.GetTodaysAppointmentsAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient history for doctor: {DoctorId}", userId);
        }

        return Page();
    }
}
