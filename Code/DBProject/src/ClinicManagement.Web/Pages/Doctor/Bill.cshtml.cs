using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<BillModel> _logger;

    public IEnumerable<AppointmentDto> TodaysAppointments { get; set; } = new List<AppointmentDto>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public BillModel(AppointmentService appointmentService, ILogger<BillModel> logger)
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
            _logger.LogError(ex, "Error loading bill page for doctor: {DoctorId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            bool result = await _appointmentService.MarkAsPaidAsync(userId.Value, appointmentId, cancellationToken);
            Success = result;
            Message = result ? "Bill marked as paid." : "Error updating bill.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment as paid: {AppointmentId}", appointmentId);
            Message = "An error occurred.";
        }

        TodaysAppointments = await _appointmentService.GetTodaysAppointmentsAsync(userId!.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            bool result = await _appointmentService.MarkAsUnpaidAsync(userId.Value, appointmentId, cancellationToken);
            Success = result;
            Message = result ? "Bill marked as unpaid." : "Error updating bill.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment as unpaid: {AppointmentId}", appointmentId);
            Message = "An error occurred.";
        }

        TodaysAppointments = await _appointmentService.GetTodaysAppointmentsAsync(userId!.Value, cancellationToken);
        return Page();
    }
}
