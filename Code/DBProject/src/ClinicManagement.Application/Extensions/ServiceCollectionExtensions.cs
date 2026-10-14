using ClinicManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Application layer services with the DI container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<PatientService>();
        services.AddScoped<DoctorService>();
        services.AddScoped<AppointmentService>();
        services.AddScoped<StaffService>();
        services.AddScoped<AdminService>();
        services.AddScoped<DepartmentService>();

        return services;
    }
}
