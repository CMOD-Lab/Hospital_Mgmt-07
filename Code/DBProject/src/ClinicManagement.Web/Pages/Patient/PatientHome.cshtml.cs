using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for the Patient Home page.
/// </summary>
public class PatientHomeModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientDto? PatientInfo { get; set; }
    public AppointmentDto? CurrentAppointment { get; set; }

    public PatientHomeModel(
        PatientService patientService,
        AppointmentService appointmentService,
        ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            PatientInfo = await _patientService.GetPatientByIdAsync(userId.Value, cancellationToken);
            CurrentAppointment = await _appointmentService.GetCurrentAppointmentAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home page for patient: {PatientId}", userId);
        }

        return Page();
    }
}
