using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for the Clinic Management System.
/// </summary>
public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
    {
    }

    public DbSet<LoginTable> LoginTable { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<OtherStaff> OtherStaff { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new LoginTableConfiguration());
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new DoctorConfiguration());
        modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        modelBuilder.ApplyConfiguration(new OtherStaffConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
    }
}
