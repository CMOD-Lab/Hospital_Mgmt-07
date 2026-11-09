using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Page model for marking doctor out of office and notifying patients.</summary>
public class DoctorOutOfOfficeModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<DoctorOutOfOfficeModel> _logger;

    public DoctorOutOfOfficeModel(AdminService adminService, ILogger<DoctorOutOfOfficeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IEnumerable<DoctorListDto> Doctors { get; set; } = Enumerable.Empty<DoctorListDto>();

    [BindProperty]
    public int DoctorId { get; set; }

    [BindProperty]
    public string OutOfOfficeDate { get; set; } = string.Empty;

    public NotificationSummaryDto? NotificationSummary { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Session-based authorization check
        var userType = HttpContext.Session.GetString("UserType");
        if (userType != "Admin")
        {
            _logger.LogWarning("Unauthorized access attempt to DoctorOutOfOffice page");
            return RedirectToPage("/Login");
        }

        try
        {
            Doctors = await _adminService.GetDoctorsAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors for out-of-office page");
            ErrorMessage = "An error occurred while loading the page.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Session-based authorization check
        var userType = HttpContext.Session.GetString("UserType");
        if (userType != "Admin")
        {
            _logger.LogWarning("Unauthorized POST attempt to DoctorOutOfOffice page");
            return RedirectToPage("/Login");
        }

        try
        {
            // Reload doctors for display
            Doctors = await _adminService.GetDoctorsAsync();

            // Validate date format
            if (!DateOnly.TryParse(OutOfOfficeDate, out var outOfOfficeDate))
            {
                ErrorMessage = "Invalid date format. Please use yyyy-MM-dd format.";
                return Page();
            }

            // Call AdminService to process out-of-office notifications
            NotificationSummary = await _adminService.MarkDoctorOutOfOfficeAsync(DoctorId, outOfOfficeDate);

            if (NotificationSummary.SuccessCount == 0 && NotificationSummary.FailureCount == 0)
            {
                ErrorMessage = "No appointments found for the selected doctor on this date.";
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing out-of-office notifications for doctor {DoctorId} on {Date}", DoctorId, OutOfOfficeDate);
            ErrorMessage = "An error occurred while processing notifications.";
            return Page();
        }
    }
}
