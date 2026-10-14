using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the Add Staff page.
/// </summary>
public class AddStaffModel : PageModel
{
    private readonly StaffService _staffService;
    private readonly ILogger<AddStaffModel> _logger;

    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public AddStaffModel(StaffService staffService, ILogger<AddStaffModel> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string Name, string BirthDate, string Phone, string Gender,
        string Address, int Salary, string Qualification, string Designation,
        CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        try
        {
            var dto = new AddStaffDto
            {
                Name = Name,
                BirthDate = BirthDate,
                Phone = Phone,
                Gender = string.IsNullOrEmpty(Gender) ? 'M' : Gender[0],
                Address = Address,
                Salary = Salary,
                Qualification = Qualification,
                Designation = Designation
            };

            bool result = await _staffService.AddStaffAsync(dto, cancellationToken);
            Success = result;
            Message = result ? "Staff member added successfully!" : "An error occurred. Please try again.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member");
            Message = "An error occurred. Please try again.";
        }

        return Page();
    }
}
