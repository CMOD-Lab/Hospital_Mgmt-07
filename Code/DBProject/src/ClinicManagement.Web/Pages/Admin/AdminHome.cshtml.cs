using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the Admin Home page.
/// </summary>
public class AdminHomeModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminHomeDto AdminInfo { get; set; } = new AdminHomeDto();

    public AdminHomeModel(AdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        // Check session
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            AdminInfo = await _adminService.GetAdminHomeInfoAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin home page");
        }

        return Page();
    }
}
