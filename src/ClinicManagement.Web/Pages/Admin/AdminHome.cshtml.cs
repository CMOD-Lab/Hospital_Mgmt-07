using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Admin home dashboard page model.</summary>
public class AdminHomeModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminHomeDto? Stats { get; set; }

    public AdminHomeModel(AdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Index");

        Stats = await _adminService.GetAdminHomeInfoAsync(ct);
        return Page();
    }
}
