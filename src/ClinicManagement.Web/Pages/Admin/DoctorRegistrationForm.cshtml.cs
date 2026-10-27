using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Doctor registration form page model.</summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public DoctorRegistrationFormModel(AdminService adminService, ILogger<DoctorRegistrationFormModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Index");

        Departments = await _adminService.GetDepartmentsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string email, string password, string birthDate,
        int deptNo, string gender, string phone, string address,
        int experience, int salary, int chargesPerVisit,
        string specialization, string qualification,
        CancellationToken ct)
    {
        Departments = await _adminService.GetDepartmentsAsync(ct);

        var dto = new AddDoctorDto
        {
            Name = name,
            Email = email,
            Password = password,
            BirthDate = birthDate,
            DeptNo = deptNo,
            Gender = string.IsNullOrEmpty(gender) ? 'M' : gender[0],
            Phone = phone,
            Address = address,
            Experience = experience,
            Salary = salary,
            ChargesPerVisit = chargesPerVisit,
            Specialization = specialization,
            Qualification = qualification
        };

        var success = await _adminService.AddDoctorAsync(dto, ct);
        IsSuccess = success;
        Message = success ? "Doctor registered successfully!" : "Error: Email may already exist or invalid data.";
        return Page();
    }
}
