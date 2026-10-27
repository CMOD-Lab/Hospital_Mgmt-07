using ClinicManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application.Extensions;

/// <summary>Extension methods for registering Application layer services.</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<AdminService>();
        services.AddScoped<PatientService>();
        services.AddScoped<DoctorService>();
        return services;
    }
}
