using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for the View Doctors page.
/// </summary>
public class ViewDoctorsModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly DepartmentService _departmentService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public IEnumerable<DoctorListDto> Doctors { get; set; } = new List<DoctorListDto>();
    public IEnumerable<DepartmentViewDto> Departments { get; set; } = new List<DepartmentViewDto>();
    public string SelectedDept { get; set; } = string.Empty;

    public ViewDoctorsModel(
        DoctorService doctorService,
        DepartmentService departmentService,
        ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string deptName = "", CancellationToken cancellationToken = default)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        SelectedDept = deptName;

        try
        {
            Departments = await _departmentService.GetAllDepartmentsAsync(cancellationToken);

            if (string.IsNullOrEmpty(deptName))
            {
                Doctors = await _doctorService.GetDoctorsAsync("", cancellationToken);
            }
            else
            {
                Doctors = await _doctorService.GetDoctorsByDepartmentAsync(deptName, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading view doctors page");
        }

        return Page();
    }
}
