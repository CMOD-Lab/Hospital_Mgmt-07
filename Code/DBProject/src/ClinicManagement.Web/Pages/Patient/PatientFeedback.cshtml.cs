using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PatientFeedbackModel> _logger;

    public AppointmentDto? PendingFeedback { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public PatientFeedbackModel(AppointmentService appointmentService, ILogger<PatientFeedbackModel> logger)
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
            PendingFeedback = await _appointmentService.GetPendingFeedbackAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedback for patient: {PatientId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        try
        {
            bool result = await _appointmentService.StoreFeedbackAsync(appointmentId, cancellationToken);
            Success = result;
            Message = result ? "Thank you for your feedback!" : "Error submitting feedback.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment: {AppointmentId}", appointmentId);
            Message = "An error occurred.";
        }

        return Page();
    }
}
