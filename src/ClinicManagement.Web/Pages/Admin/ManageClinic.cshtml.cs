using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Manage clinic page model - view/delete doctors, patients, staff.</summary>
public class ManageClinicModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<ManageClinicModel> _logger;

    public IEnumerable<DoctorListDto> Doctors { get; set; } = Enumerable.Empty<DoctorListDto>();
    public IEnumerable<PatientListDto> Patients { get; set; } = Enumerable.Empty<PatientListDto>();
    public IEnumerable<StaffListDto> Staff { get; set; } = Enumerable.Empty<StaffListDto>();
    public string? DoctorSearch { get; set; }
    public string? Message { get; set; }

    public ManageClinicModel(AdminService adminService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? doctorSearch, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Index");

        DoctorSearch = doctorSearch ?? string.Empty;
        Doctors = await _adminService.GetDoctorsAsync(DoctorSearch, ct);
        Patients = await _adminService.GetPatientsAsync(string.Empty, ct);
        Staff = await _adminService.GetStaffAsync(string.Empty, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId, CancellationToken ct)
    {
        var success = await _adminService.DeleteDoctorAsync(doctorId, ct);
        Message = success ? "Doctor removed successfully." : "Error removing doctor.";
        return await OnGetAsync(string.Empty, ct);
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId, CancellationToken ct)
    {
        var success = await _adminService.DeleteStaffAsync(staffId, ct);
        Message = success ? "Staff member deleted successfully." : "Error deleting staff.";
        return await OnGetAsync(string.Empty, ct);
    }
}
