using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service for admin-related operations.
/// </summary>
public class AdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IDepartmentRepository departmentRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets admin home page statistics.
    /// </summary>
    public async Task<AdminHomeDto> GetAdminHomeInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving admin home information");

            var doctors = await _doctorRepository.GetActiveAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);

            double totalIncome = appointments
                .Where(a => a.BillStatus == "Paid" && a.BillAmount.HasValue)
                .Sum(a => a.BillAmount!.Value);

            var departmentViews = departments.Select(d => new DepartmentViewDto
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName,
                Description = d.Description,
                DoctorCount = d.Doctors?.Count(doc => doc.Status == 1) ?? 0
            });

            var recentAppointments = appointments
                .OrderByDescending(a => a.Date)
                .Take(10)
                .Select(a => new AppointmentDto
                {
                    AppointId = a.AppointId,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor?.Name,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.Name,
                    Date = a.Date,
                    AppointmentStatus = a.AppointmentStatus,
                    BillAmount = a.BillAmount,
                    BillStatus = a.BillStatus
                });

            return new AdminHomeDto
            {
                TotalDoctors = doctors.Count(),
                TotalPatients = patients.Count(),
                TotalIncome = totalIncome,
                Departments = departmentViews,
                RecentAppointments = recentAppointments
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin home information");
            return new AdminHomeDto();
        }
    }
}
