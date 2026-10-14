using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public IEnumerable<AppointmentDto> TreatmentHistory { get; set; } = new List<AppointmentDto>();

    public TreatmentHistoryModel(AppointmentService appointmentService, ILogger<TreatmentHistoryModel> logger)
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
            TreatmentHistory = await _appointmentService.GetTreatmentHistoryAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history for patient: {PatientId}", userId);
        }

        return Page();
    }
}
