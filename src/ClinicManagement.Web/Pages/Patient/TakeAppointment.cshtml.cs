using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TakeAppointmentModel : PageModel
{
    private readonly PatientService _patientService;
    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();
    public IEnumerable<DoctorListDto> Doctors { get; set; } = Enumerable.Empty<DoctorListDto>();
    public string? SelectedDept { get; set; }
    public string? Message { get; set; }

    public TakeAppointmentModel(PatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(string? selectedDept, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToPage("/Index");

        Departments = await _patientService.GetDepartmentsAsync(ct);
        SelectedDept = selectedDept;

        if (!string.IsNullOrEmpty(selectedDept))
            Doctors = await _patientService.GetDoctorsByDepartmentAsync(selectedDept, ct);

        return Page();
    }
}
