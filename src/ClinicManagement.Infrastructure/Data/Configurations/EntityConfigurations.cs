using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

public class LoginTableConfiguration : IEntityTypeConfiguration<LoginTable>
{
    public void Configure(EntityTypeBuilder<LoginTable> builder)
    {
        builder.ToTable("LoginTable");
        builder.HasKey(l => l.LoginId);
        builder.Property(l => l.LoginId).HasColumnName("LoginID").ValueGeneratedOnAdd();
        builder.Property(l => l.Password).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Email).HasMaxLength(30).IsRequired();
        builder.Property(l => l.Type).HasColumnName("Type").IsRequired();
        builder.HasIndex(l => l.Email).IsUnique();
    }
}

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patient");
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).HasColumnName("PatientID").ValueGeneratedNever();
        builder.Property(p => p.Name).HasMaxLength(30).IsRequired();
        builder.Property(p => p.Phone).HasMaxLength(11).IsFixedLength();
        builder.Property(p => p.Address).HasMaxLength(40);
        builder.Property(p => p.BirthDate).IsRequired();
        builder.Property(p => p.Gender).HasMaxLength(1).IsFixedLength().IsRequired();

        builder.HasOne(p => p.Login)
               .WithOne(l => l.Patient)
               .HasForeignKey<Patient>(p => p.PatientId);
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Department");
        builder.HasKey(d => d.DeptNo);
        builder.Property(d => d.DeptNo).HasColumnName("DeptNo").ValueGeneratedNever();
        builder.Property(d => d.DeptName).HasMaxLength(30).IsRequired();
        builder.HasIndex(d => d.DeptName).IsUnique();
        builder.Property(d => d.Description).HasMaxLength(1000);
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctor");
        builder.HasKey(d => d.DoctorId);
        builder.Property(d => d.DoctorId).HasColumnName("DoctorID").ValueGeneratedNever();
        builder.Property(d => d.Name).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Phone).HasMaxLength(11).IsFixedLength();
        builder.Property(d => d.Address).HasMaxLength(40);
        builder.Property(d => d.BirthDate).IsRequired();
        builder.Property(d => d.Gender).HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(d => d.ChargesPerVisit).HasColumnName("Charges_Per_Visit").IsRequired();
        builder.Property(d => d.MonthlySalary).HasColumnName("MonthlySalary");
        builder.Property(d => d.ReputeIndex).HasColumnName("ReputeIndex");
        builder.Property(d => d.PatientsTreated).HasColumnName("Patients_Treated").HasDefaultValue(0).IsRequired();
        builder.Property(d => d.Qualification).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Specialization).HasMaxLength(100).HasColumnName("Specialization");
        builder.Property(d => d.WorkExperience).HasColumnName("Work_Experience");
        builder.Property(d => d.Status).IsRequired();

        builder.HasOne(d => d.Department)
               .WithMany(dept => dept.Doctors)
               .HasForeignKey(d => d.DeptNo);

        builder.HasOne(d => d.Login)
               .WithOne(l => l.Doctor)
               .HasForeignKey<Doctor>(d => d.DoctorId);
    }
}

public class OtherStaffConfiguration : IEntityTypeConfiguration<OtherStaff>
{
    public void Configure(EntityTypeBuilder<OtherStaff> builder)
    {
        builder.ToTable("OtherStaff");
        builder.HasKey(s => s.StaffId);
        builder.Property(s => s.StaffId).HasColumnName("StaffID").ValueGeneratedOnAdd();
        builder.Property(s => s.Name).HasMaxLength(30).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(11).IsFixedLength();
        builder.Property(s => s.Address).HasMaxLength(30);
        builder.Property(s => s.Designation).HasMaxLength(15).IsRequired();
        builder.Property(s => s.Gender).HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(s => s.HighestQualification).HasMaxLength(50).HasColumnName("Highest_Qualification");
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointment");
        builder.HasKey(a => a.AppointId);
        builder.Property(a => a.AppointId).HasColumnName("AppointID").ValueGeneratedOnAdd();
        builder.Property(a => a.AppointmentStatus).HasColumnName("Appointment_Status");
        builder.Property(a => a.BillAmount).HasColumnName("Bill_Amount");
        builder.Property(a => a.BillStatus).HasColumnName("Bill_Status").HasMaxLength(10)
               .HasConversion(
                   v => v.HasValue ? v.Value.ToString() : null,
                   v => v != null ? Enum.Parse<Domain.Enums.BillStatus>(v) : (Domain.Enums.BillStatus?)null);
        builder.Property(a => a.DoctorNotification).HasColumnName("DoctorNotification");
        builder.Property(a => a.PatientNotification).HasColumnName("PatientNotification");
        builder.Property(a => a.FeedbackStatus).HasColumnName("FeedbackStatus");
        builder.Property(a => a.Disease).HasMaxLength(100);
        builder.Property(a => a.Progress).HasMaxLength(100);
        builder.Property(a => a.Prescription).HasMaxLength(100);

        builder.HasOne(a => a.Doctor)
               .WithMany(d => d.Appointments)
               .HasForeignKey(a => a.DoctorId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
