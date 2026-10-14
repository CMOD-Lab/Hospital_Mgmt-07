using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class DoctorHomeModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<DoctorHomeModel> _logger;

    public DoctorDto? DoctorInfo { get; set; }
    public IEnumerable<AppointmentDto> TodaysAppointments { get; set; } = new List<AppointmentDto>();

    public DoctorHomeModel(
        DoctorService doctorService,
        AppointmentService appointmentService,
        ILogger<DoctorHomeModel> logger)
    {
        _doctorService = doctorService;
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 2) return RedirectToPage("/SignUp");

        try
        {
            DoctorInfo = await _doctorService.GetDoctorByIdAsync(userId.Value, cancellationToken);
            TodaysAppointments = await _appointmentService.GetTodaysAppointmentsAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home page for doctor: {DoctorId}", userId);
        }

        return Page();
    }
}
