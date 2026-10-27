using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class DoctorHomeModel : PageModel
{
    private readonly DoctorService _doctorService;
    public DoctorProfileDto? DoctorInfo { get; set; }

    public DoctorHomeModel(DoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");
        DoctorInfo = await _doctorService.GetDoctorInfoAsync(userId.Value, ct);
        return Page();
    }
}
