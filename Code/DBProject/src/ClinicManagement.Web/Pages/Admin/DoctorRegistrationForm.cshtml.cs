using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the Doctor Registration Form.
/// </summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly DepartmentService _departmentService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public IEnumerable<DepartmentViewDto> Departments { get; set; } = new List<DepartmentViewDto>();

    public DoctorRegistrationFormModel(
        DoctorService doctorService,
        DepartmentService departmentService,
        ILogger<DoctorRegistrationFormModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        Departments = await _departmentService.GetAllDepartmentsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string Name, string Email, string Password, string BirthDate,
        int DeptNo, string Phone, string Gender, string Address,
        int WorkExperience, int Salary, int ChargesPerVisit,
        string Specialization, string Qualification,
        CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        try
        {
            var dto = new AddDoctorDto
            {
                Name = Name,
                Email = Email,
                Password = Password,
                BirthDate = BirthDate,
                DeptNo = DeptNo,
                Phone = Phone,
                Gender = string.IsNullOrEmpty(Gender) ? 'M' : Gender[0],
                Address = Address,
                WorkExperience = WorkExperience,
                Salary = Salary,
                ChargesPerVisit = ChargesPerVisit,
                Specialization = Specialization,
                Qualification = Qualification
            };

            bool result = await _doctorService.AddDoctorAsync(dto, cancellationToken);

            if (result)
            {
                Success = true;
                Message = "Doctor registered successfully!";
            }
            else
            {
                Success = false;
                Message = "Email already exists or an error occurred. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor");
            Message = "An error occurred. Please try again.";
        }

        Departments = await _departmentService.GetAllDepartmentsAsync(cancellationToken);
        return Page();
    }
}
