using ClinicManagement.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.Domain;

/// <summary>
/// Unit tests for Domain Entities.
/// </summary>
public class DomainEntityTests
{
    // ─── Doctor Entity ────────────────────────────────────────────────────────

    [Fact]
    public void Doctor_DefaultConstructor_InitializesCollections()
    {
        // Act
        var doctor = new Doctor();

        // Assert
        doctor.Appointments.Should().NotBeNull();
        doctor.Appointments.Should().BeEmpty();
        doctor.Name.Should().Be(string.Empty);
        doctor.Qualification.Should().Be(string.Empty);
        doctor.PatientsTreated.Should().Be(0);
    }

    [Fact]
    public void Doctor_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var doctor = new Doctor
        {
            DoctorId = 1,
            Name = "Dr. Smith",
            Phone = "9876543210",
            Address = "123 Main St",
            BirthDate = new DateTime(1975, 6, 15),
            Gender = 'M',
            DeptNo = 2,
            ChargesPerVisit = 500.0,
            MonthlySalary = 80000.0,
            ReputeIndex = 4.5,
            PatientsTreated = 150,
            Qualification = "MBBS, MD",
            Specialization = "Cardiology",
            WorkExperience = 20,
            Status = 1
        };

        // Assert
        doctor.DoctorId.Should().Be(1);
        doctor.Name.Should().Be("Dr. Smith");
        doctor.Phone.Should().Be("9876543210");
        doctor.Address.Should().Be("123 Main St");
        doctor.BirthDate.Should().Be(new DateTime(1975, 6, 15));
        doctor.Gender.Should().Be('M');
        doctor.DeptNo.Should().Be(2);
        doctor.ChargesPerVisit.Should().Be(500.0);
        doctor.MonthlySalary.Should().Be(80000.0);
        doctor.ReputeIndex.Should().Be(4.5);
        doctor.PatientsTreated.Should().Be(150);
        doctor.Qualification.Should().Be("MBBS, MD");
        doctor.Specialization.Should().Be("Cardiology");
        doctor.WorkExperience.Should().Be(20);
        doctor.Status.Should().Be(1);
    }

    [Fact]
    public void Doctor_NullableProperties_CanBeNull()
    {
        // Arrange & Act
        var doctor = new Doctor
        {
            DoctorId = 1,
            Name = "Dr. Null",
            BirthDate = DateTime.Now,
            Gender = 'M',
            ChargesPerVisit = 100.0,
            Qualification = "MBBS",
            Status = 1,
            Phone = null,
            Address = null,
            MonthlySalary = null,
            ReputeIndex = null,
            Specialization = null,
            WorkExperience = null,
            Department = null,
            Login = null
        };

        // Assert
        doctor.Phone.Should().BeNull();
        doctor.Address.Should().BeNull();
        doctor.MonthlySalary.Should().BeNull();
        doctor.ReputeIndex.Should().BeNull();
        doctor.Specialization.Should().BeNull();
        doctor.WorkExperience.Should().BeNull();
        doctor.Department.Should().BeNull();
        doctor.Login.Should().BeNull();
    }

    // ─── Patient Entity ───────────────────────────────────────────────────────

    [Fact]
    public void Patient_DefaultConstructor_InitializesCollections()
    {
        // Act
        var patient = new Patient();

        // Assert
        patient.Appointments.Should().NotBeNull();
        patient.Appointments.Should().BeEmpty();
        patient.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void Patient_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var patient = new Patient
        {
            PatientId = 10,
            Name = "John Doe",
            Phone = "1234567890",
            Address = "456 Oak Ave",
            BirthDate = new DateTime(1990, 3, 25),
            Gender = 'M'
        };

        // Assert
        patient.PatientId.Should().Be(10);
        patient.Name.Should().Be("John Doe");
        patient.Phone.Should().Be("1234567890");
        patient.Address.Should().Be("456 Oak Ave");
        patient.BirthDate.Should().Be(new DateTime(1990, 3, 25));
        patient.Gender.Should().Be('M');
    }

    [Fact]
    public void Patient_NullableProperties_CanBeNull()
    {
        // Arrange & Act
        var patient = new Patient
        {
            PatientId = 1,
            Name = "Jane",
            BirthDate = DateTime.Now,
            Gender = 'F',
            Phone = null,
            Address = null,
            Login = null
        };

        // Assert
        patient.Phone.Should().BeNull();
        patient.Address.Should().BeNull();
        patient.Login.Should().BeNull();
    }

    // ─── Appointment Entity ───────────────────────────────────────────────────

    [Fact]
    public void Appointment_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var date = new DateTime(2024, 7, 10, 9, 0, 0);
        var appointment = new Appointment
        {
            AppointId = 5,
            DoctorId = 1,
            PatientId = 2,
            Date = date,
            AppointmentStatus = 1,
            BillAmount = 600.0,
            BillStatus = "Paid",
            DoctorNotification = 1,
            PatientNotification = 2,
            FeedbackStatus = 2,
            Disease = "Diabetes",
            Progress = "Controlled",
            Prescription = "Metformin 500mg"
        };

        // Assert
        appointment.AppointId.Should().Be(5);
        appointment.DoctorId.Should().Be(1);
        appointment.PatientId.Should().Be(2);
        appointment.Date.Should().Be(date);
        appointment.AppointmentStatus.Should().Be(1);
        appointment.BillAmount.Should().Be(600.0);
        appointment.BillStatus.Should().Be("Paid");
        appointment.DoctorNotification.Should().Be(1);
        appointment.PatientNotification.Should().Be(2);
        appointment.FeedbackStatus.Should().Be(2);
        appointment.Disease.Should().Be("Diabetes");
        appointment.Progress.Should().Be("Controlled");
        appointment.Prescription.Should().Be("Metformin 500mg");
    }

    [Fact]
    public void Appointment_NullableProperties_CanBeNull()
    {
        // Arrange & Act
        var appointment = new Appointment
        {
            AppointId = 1,
            AppointmentStatus = 2,
            DoctorId = null,
            PatientId = null,
            Date = null,
            BillAmount = null,
            BillStatus = null,
            DoctorNotification = null,
            PatientNotification = null,
            FeedbackStatus = null,
            Disease = null,
            Progress = null,
            Prescription = null,
            Doctor = null,
            Patient = null
        };

        // Assert
        appointment.DoctorId.Should().BeNull();
        appointment.PatientId.Should().BeNull();
        appointment.Date.Should().BeNull();
        appointment.BillAmount.Should().BeNull();
        appointment.BillStatus.Should().BeNull();
        appointment.DoctorNotification.Should().BeNull();
        appointment.PatientNotification.Should().BeNull();
        appointment.FeedbackStatus.Should().BeNull();
        appointment.Disease.Should().BeNull();
        appointment.Progress.Should().BeNull();
        appointment.Prescription.Should().BeNull();
        appointment.Doctor.Should().BeNull();
        appointment.Patient.Should().BeNull();
    }

    // ─── Department Entity ────────────────────────────────────────────────────

    [Fact]
    public void Department_DefaultConstructor_InitializesCollections()
    {
        // Act
        var department = new Department();

        // Assert
        department.Doctors.Should().NotBeNull();
        department.Doctors.Should().BeEmpty();
        department.DeptName.Should().Be(string.Empty);
    }

    [Fact]
    public void Department_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var department = new Department
        {
            DeptNo = 3,
            DeptName = "Pediatrics",
            Description = "Child care"
        };

        // Assert
        department.DeptNo.Should().Be(3);
        department.DeptName.Should().Be("Pediatrics");
        department.Description.Should().Be("Child care");
    }

    [Fact]
    public void Department_CanAddDoctors()
    {
        // Arrange
        var department = new Department { DeptNo = 1, DeptName = "ENT" };
        var doctor = new Doctor { DoctorId = 1, Name = "Dr. Ear", Status = 1 };

        // Act
        department.Doctors.Add(doctor);

        // Assert
        department.Doctors.Should().HaveCount(1);
        department.Doctors.First().Name.Should().Be("Dr. Ear");
    }

    // ─── LoginTable Entity ────────────────────────────────────────────────────

    [Fact]
    public void LoginTable_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var login = new LoginTable
        {
            LoginId = 1,
            Email = "user@clinic.com",
            Password = "securepass",
            Type = 1
        };

        // Assert
        login.LoginId.Should().Be(1);
        login.Email.Should().Be("user@clinic.com");
        login.Password.Should().Be("securepass");
        login.Type.Should().Be(1);
    }

    [Fact]
    public void LoginTable_DefaultConstructor_HasEmptyStrings()
    {
        // Act
        var login = new LoginTable();

        // Assert
        login.Password.Should().Be(string.Empty);
        login.Email.Should().Be(string.Empty);
    }

    [Fact]
    public void LoginTable_NullableNavigationProperties_CanBeNull()
    {
        // Arrange & Act
        var login = new LoginTable
        {
            LoginId = 1,
            Email = "test@test.com",
            Password = "pass",
            Type = 3,
            Patient = null,
            Doctor = null
        };

        // Assert
        login.Patient.Should().BeNull();
        login.Doctor.Should().BeNull();
    }

    // ─── OtherStaff Entity ────────────────────────────────────────────────────

    [Fact]
    public void OtherStaff_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var staff = new OtherStaff
        {
            StaffId = 7,
            Name = "Alice Johnson",
            Phone = "5551234567",
            Address = "321 Birch Rd",
            Designation = "Pharmacist",
            Gender = 'F',
            BirthDate = new DateTime(1988, 11, 5),
            HighestQualification = "B.Pharm",
            Salary = 45000.0
        };

        // Assert
        staff.StaffId.Should().Be(7);
        staff.Name.Should().Be("Alice Johnson");
        staff.Phone.Should().Be("5551234567");
        staff.Address.Should().Be("321 Birch Rd");
        staff.Designation.Should().Be("Pharmacist");
        staff.Gender.Should().Be('F');
        staff.BirthDate.Should().Be(new DateTime(1988, 11, 5));
        staff.HighestQualification.Should().Be("B.Pharm");
        staff.Salary.Should().Be(45000.0);
    }

    [Fact]
    public void OtherStaff_DefaultConstructor_HasEmptyStrings()
    {
        // Act
        var staff = new OtherStaff();

        // Assert
        staff.Name.Should().Be(string.Empty);
        staff.Designation.Should().Be(string.Empty);
    }

    [Fact]
    public void OtherStaff_NullableProperties_CanBeNull()
    {
        // Arrange & Act
        var staff = new OtherStaff
        {
            StaffId = 1,
            Name = "Bob",
            Designation = "Nurse",
            Gender = 'M',
            Phone = null,
            Address = null,
            BirthDate = null,
            HighestQualification = null,
            Salary = null
        };

        // Assert
        staff.Phone.Should().BeNull();
        staff.Address.Should().BeNull();
        staff.BirthDate.Should().BeNull();
        staff.HighestQualification.Should().BeNull();
        staff.Salary.Should().BeNull();
    }
}
