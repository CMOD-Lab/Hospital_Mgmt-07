using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the Manage Clinic page.
/// </summary>
public class ManageClinicModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly PatientService _patientService;
    private readonly StaffService _staffService;
    private readonly ILogger<ManageClinicModel> _logger;

    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string SearchType { get; set; } = "doctor";
    public string SearchQuery { get; set; } = string.Empty;
    public IEnumerable<DoctorListDto> Doctors { get; set; } = new List<DoctorListDto>();
    public IEnumerable<PatientListDto> Patients { get; set; } = new List<PatientListDto>();
    public IEnumerable<StaffListDto> Staff { get; set; } = new List<StaffListDto>();

    public ManageClinicModel(
        DoctorService doctorService,
        PatientService patientService,
        StaffService staffService,
        ILogger<ManageClinicModel> logger)
    {
        _doctorService = doctorService;
        _patientService = patientService;
        _staffService = staffService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(
        string searchType = "doctor",
        string searchQuery = "",
        CancellationToken cancellationToken = default)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        SearchType = searchType;
        SearchQuery = searchQuery;

        try
        {
            switch (searchType)
            {
                case "patient":
                    Patients = await _patientService.GetPatientsAsync(searchQuery, cancellationToken);
                    break;
                case "staff":
                    Staff = await _staffService.GetStaffAsync(searchQuery, cancellationToken);
                    break;
                default:
                    Doctors = await _doctorService.GetDoctorsAsync(searchQuery, cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manage clinic page");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        try
        {
            bool result = await _doctorService.DeleteDoctorAsync(doctorId, cancellationToken);
            Success = result;
            Message = result ? "Doctor removed successfully." : "Error removing doctor.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor: {DoctorId}", doctorId);
            Message = "An error occurred.";
        }

        Doctors = await _doctorService.GetDoctorsAsync("", cancellationToken);
        SearchType = "doctor";
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId, CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        try
        {
            bool result = await _staffService.DeleteStaffAsync(staffId, cancellationToken);
            Success = result;
            Message = result ? "Staff member removed successfully." : "Error removing staff member.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff: {StaffId}", staffId);
            Message = "An error occurred.";
        }

        Staff = await _staffService.GetStaffAsync("", cancellationToken);
        SearchType = "staff";
        return Page();
    }
}
