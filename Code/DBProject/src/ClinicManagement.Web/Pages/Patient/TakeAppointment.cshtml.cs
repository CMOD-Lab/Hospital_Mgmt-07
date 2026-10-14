using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TakeAppointmentModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly DoctorService _doctorService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public DoctorDto? Doctor { get; set; }
    public IEnumerable<AppointmentDto> FreeSlots { get; set; } = new List<AppointmentDto>();
    public int DoctorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public TakeAppointmentModel(
        AppointmentService appointmentService,
        DoctorService doctorService,
        ILogger<TakeAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        DoctorId = doctorId;

        try
        {
            Doctor = await _doctorService.GetDoctorByIdAsync(doctorId, cancellationToken);
            FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading take appointment page");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int freeSlotId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");

        DoctorId = doctorId;

        try
        {
            var dto = new CreateAppointmentDto
            {
                DoctorId = doctorId,
                PatientId = userId.Value,
                FreeSlotId = freeSlotId
            };

            bool result = await _appointmentService.CreateAppointmentAsync(dto, cancellationToken);
            Success = result;
            Message = result ? "Appointment request sent successfully!" : "Error booking appointment.";

            if (result)
            {
                return RedirectToPage("/Patient/AppointmentRequestSent");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            Message = "An error occurred. Please try again.";
        }

        Doctor = await _doctorService.GetDoctorByIdAsync(doctorId, cancellationToken);
        FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, userId!.Value, cancellationToken);
        return Page();
    }
}
