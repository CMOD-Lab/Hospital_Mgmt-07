using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<BillsHistoryModel> _logger;

    public IEnumerable<AppointmentDto> BillHistory { get; set; } = new List<AppointmentDto>();

    public BillsHistoryModel(AppointmentService appointmentService, ILogger<BillsHistoryModel> logger)
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
            BillHistory = await _appointmentService.GetBillHistoryAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bill history for patient: {PatientId}", userId);
        }

        return Page();
    }
}
