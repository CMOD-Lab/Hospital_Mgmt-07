using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class AppointmentRequestSentModel : PageModel
{
    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 1) return RedirectToPage("/SignUp");
        return Page();
    }
}
