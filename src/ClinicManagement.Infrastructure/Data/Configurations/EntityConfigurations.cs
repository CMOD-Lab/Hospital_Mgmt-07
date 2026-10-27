using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Data.Configurations;

public class LoginTableConfiguration : IEntityTypeConfiguration<LoginTable>
{
    public void Configure(EntityTypeBuilder<LoginTable> builder)
    {
        builder.ToTable("login_table");
        builder.HasKey(l => l.LoginId);
        builder.Property(l => l.LoginId).HasColumnName("login_id").ValueGeneratedOnAdd();
        builder.Property(l => l.Password).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Email).HasMaxLength(30).IsRequired();
        builder.Property(l => l.Type).HasColumnName("type").IsRequired();
        builder.HasIndex(l => l.Email).IsUnique();
    }
}

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patient");
        builder.HasKey(p => p.PatientId);
        builder.Property(p => p.PatientId).HasColumnName("patient_id").ValueGeneratedNever();
        builder.Property(p => p.Name).HasMaxLength(30).IsRequired();
        builder.Property(p => p.Phone).HasMaxLength(11).HasColumnType("char(11)");
        builder.Property(p => p.Address).HasMaxLength(40);
        builder.Property(p => p.BirthDate).HasColumnType("date").IsRequired();
        builder.Property(p => p.Gender).HasMaxLength(1).HasColumnType("char(1)").IsRequired();

        builder.HasOne(p => p.Login)
               .WithOne(l => l.Patient)
               .HasForeignKey<Patient>(p => p.PatientId);
    }
}

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");
        builder.HasKey(d => d.DeptNo);
        builder.Property(d => d.DeptNo).HasColumnName("dept_no").ValueGeneratedNever();
        builder.Property(d => d.DeptName).HasMaxLength(30).IsRequired();
        builder.HasIndex(d => d.DeptName).IsUnique();
        builder.Property(d => d.Description).HasMaxLength(1000);
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctor");
        builder.HasKey(d => d.DoctorId);
        builder.Property(d => d.DoctorId).HasColumnName("doctor_id").ValueGeneratedNever();
        builder.Property(d => d.Name).HasMaxLength(30).IsRequired();
        builder.Property(d => d.Phone).HasMaxLength(11).HasColumnType("char(11)");
        builder.Property(d => d.Address).HasMaxLength(40);
        builder.Property(d => d.BirthDate).HasColumnType("date").IsRequired();
        builder.Property(d => d.Gender).HasMaxLength(1).HasColumnType("char(1)").IsRequired();
        builder.Property(d => d.ChargesPerVisit).HasColumnName("charges_per_visit").IsRequired();
        builder.Property(d => d.MonthlySalary).HasColumnName("monthly_salary");
        builder.Property(d => d.ReputeIndex).HasColumnName("repute_index");
        builder.Property(d => d.PatientsTreated).HasColumnName("patients_treated").HasDefaultValue(0).IsRequired();
        builder.Property(d => d.Qualification).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Specialization).HasMaxLength(100).HasColumnName("specialization");
        builder.Property(d => d.WorkExperience).HasColumnName("work_experience");
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
        builder.ToTable("other_staff");
        builder.HasKey(s => s.StaffId);
        builder.Property(s => s.StaffId).HasColumnName("staff_id").ValueGeneratedOnAdd();
        builder.Property(s => s.Name).HasMaxLength(30).IsRequired();
        builder.Property(s => s.Phone).HasMaxLength(11).HasColumnType("char(11)");
        builder.Property(s => s.Address).HasMaxLength(30);
        builder.Property(s => s.Designation).HasMaxLength(15).IsRequired();
        builder.Property(s => s.Gender).HasMaxLength(1).HasColumnType("char(1)").IsRequired();
        builder.Property(s => s.HighestQualification).HasMaxLength(50).HasColumnName("highest_qualification");
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointment");
        builder.HasKey(a => a.AppointId);
        builder.Property(a => a.AppointId).HasColumnName("appoint_id").ValueGeneratedOnAdd();
        builder.Property(a => a.AppointmentStatus).HasColumnName("appointment_status");
        builder.Property(a => a.BillAmount).HasColumnName("bill_amount");
        builder.Property(a => a.BillStatus).HasColumnName("bill_status").HasMaxLength(10)
               .HasConversion(
                   v => v.HasValue ? v.Value.ToString() : null,
                   v => v != null ? Enum.Parse<Domain.Enums.BillStatus>(v) : (Domain.Enums.BillStatus?)null);
        builder.Property(a => a.DoctorNotification).HasColumnName("doctor_notification");
        builder.Property(a => a.PatientNotification).HasColumnName("patient_notification");
        builder.Property(a => a.FeedbackStatus).HasColumnName("feedback_status");
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
