using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Add staff page model.</summary>
public class AddStaffModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public AddStaffModel(AdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string birthDate, string phone, string gender,
        string address, int salary, string qualification, string designation,
        CancellationToken ct)
    {
        var dto = new AddStaffDto
        {
            Name = name,
            BirthDate = birthDate,
            Phone = phone,
            Gender = string.IsNullOrEmpty(gender) ? 'M' : gender[0],
            Address = address,
            Salary = salary,
            Qualification = qualification,
            Designation = designation
        };

        var success = await _adminService.AddStaffAsync(dto, ct);
        IsSuccess = success;
        Message = success ? "Staff member added successfully!" : "Error adding staff member.";
        return Page();
    }
}
