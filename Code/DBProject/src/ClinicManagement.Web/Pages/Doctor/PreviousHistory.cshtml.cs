using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PreviousHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PreviousHistoryModel> _logger;

    public IEnumerable<AppointmentDto> PatientHistory { get; set; } = new List<AppointmentDto>();

    public PreviousHistoryModel(AppointmentService appointmentService, ILogger<PreviousHistoryModel> logger)
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
            PatientHistory = await _appointmentService.GetPatientHistoryAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading previous history for doctor: {DoctorId}", userId);
        }

        return Page();
    }
}
