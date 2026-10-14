using ClinicManagement.Application.DTOs;
using FluentAssertions;
using Xunit;

namespace ClinicManagement.UnitTests.DTOs;

/// <summary>
/// Unit tests for Application DTOs.
/// </summary>
public class DtoTests
{
    // ─── AppointmentDto ───────────────────────────────────────────────────────

    [Fact]
    public void AppointmentDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new AppointmentDto
        {
            AppointId = 1,
            DoctorId = 10,
            DoctorName = "Dr. Smith",
            PatientId = 20,
            PatientName = "John Doe",
            Date = new DateTime(2024, 6, 15),
            Timings = "10:00 AM",
            AppointmentStatus = 1,
            BillAmount = 500.0,
            BillStatus = "Paid",
            Disease = "Flu",
            Progress = "Recovering",
            Prescription = "Rest",
            FeedbackStatus = 1
        };

        // Assert
        dto.AppointId.Should().Be(1);
        dto.DoctorId.Should().Be(10);
        dto.DoctorName.Should().Be("Dr. Smith");
        dto.PatientId.Should().Be(20);
        dto.PatientName.Should().Be("John Doe");
        dto.AppointmentStatus.Should().Be(1);
        dto.BillAmount.Should().Be(500.0);
        dto.BillStatus.Should().Be("Paid");
        dto.Disease.Should().Be("Flu");
        dto.Progress.Should().Be("Recovering");
        dto.Prescription.Should().Be("Rest");
        dto.FeedbackStatus.Should().Be(1);
    }

    [Fact]
    public void AppointmentDto_DefaultConstructor_HasDefaultValues()
    {
        // Act
        var dto = new AppointmentDto();

        // Assert
        dto.AppointId.Should().Be(0);
        dto.AppointmentStatus.Should().Be(0);
        dto.DoctorName.Should().BeNull();
        dto.PatientName.Should().BeNull();
    }

    // ─── CreateAppointmentDto ─────────────────────────────────────────────────

    [Fact]
    public void CreateAppointmentDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new CreateAppointmentDto
        {
            DoctorId = 5,
            PatientId = 15,
            FreeSlotId = 3
        };

        // Assert
        dto.DoctorId.Should().Be(5);
        dto.PatientId.Should().Be(15);
        dto.FreeSlotId.Should().Be(3);
    }

    // ─── UpdatePrescriptionDto ────────────────────────────────────────────────

    [Fact]
    public void UpdatePrescriptionDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new UpdatePrescriptionDto
        {
            AppointmentId = 7,
            DoctorId = 2,
            Disease = "Hypertension",
            Progress = "Stable",
            Prescription = "Amlodipine 5mg"
        };

        // Assert
        dto.AppointmentId.Should().Be(7);
        dto.DoctorId.Should().Be(2);
        dto.Disease.Should().Be("Hypertension");
        dto.Progress.Should().Be("Stable");
        dto.Prescription.Should().Be("Amlodipine 5mg");
    }

    [Fact]
    public void UpdatePrescriptionDto_DefaultConstructor_HasEmptyStrings()
    {
        // Act
        var dto = new UpdatePrescriptionDto();

        // Assert
        dto.Disease.Should().Be(string.Empty);
        dto.Progress.Should().Be(string.Empty);
        dto.Prescription.Should().Be(string.Empty);
    }

    // ─── AdminHomeDto ─────────────────────────────────────────────────────────

    [Fact]
    public void AdminHomeDto_DefaultConstructor_InitializesCollections()
    {
        // Act
        var dto = new AdminHomeDto();

        // Assert
        dto.Departments.Should().NotBeNull();
        dto.RecentAppointments.Should().NotBeNull();
        dto.TotalDoctors.Should().Be(0);
        dto.TotalPatients.Should().Be(0);
        dto.TotalIncome.Should().Be(0.0);
    }

    [Fact]
    public void AdminHomeDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new AdminHomeDto
        {
            TotalDoctors = 10,
            TotalPatients = 50,
            TotalIncome = 25000.0
        };

        // Assert
        dto.TotalDoctors.Should().Be(10);
        dto.TotalPatients.Should().Be(50);
        dto.TotalIncome.Should().Be(25000.0);
    }

    // ─── DepartmentViewDto ────────────────────────────────────────────────────

    [Fact]
    public void DepartmentViewDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new DepartmentViewDto
        {
            DeptNo = 2,
            DeptName = "Cardiology",
            Description = "Heart care",
            DoctorCount = 5
        };

        // Assert
        dto.DeptNo.Should().Be(2);
        dto.DeptName.Should().Be("Cardiology");
        dto.Description.Should().Be("Heart care");
        dto.DoctorCount.Should().Be(5);
    }

    [Fact]
    public void DepartmentViewDto_DefaultConstructor_HasEmptyStrings()
    {
        // Act
        var dto = new DepartmentViewDto();

        // Assert
        dto.DeptName.Should().Be(string.Empty);
        dto.DoctorCount.Should().Be(0);
    }

    // ─── FreeSlotDto ──────────────────────────────────────────────────────────

    [Fact]
    public void FreeSlotDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var date = new DateTime(2024, 8, 20);
        var dto = new FreeSlotDto
        {
            SlotId = 3,
            Timings = "02:00 PM",
            Date = date
        };

        // Assert
        dto.SlotId.Should().Be(3);
        dto.Timings.Should().Be("02:00 PM");
        dto.Date.Should().Be(date);
    }

    [Fact]
    public void FreeSlotDto_DefaultConstructor_HasEmptyTimings()
    {
        // Act
        var dto = new FreeSlotDto();

        // Assert
        dto.Timings.Should().Be(string.Empty);
    }

    // ─── DoctorDto ────────────────────────────────────────────────────────────

    [Fact]
    public void DoctorDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new DoctorDto
        {
            DoctorId = 1,
            Name = "Dr. House",
            Phone = "9876543210",
            Gender = "M",
            ChargesPerVisit = 600.0,
            ReputeIndex = 4.8,
            PatientsTreated = 200,
            Qualification = "MBBS, MD",
            Specialization = "Diagnostics",
            WorkExperience = 25,
            Age = 50,
            Department = "General"
        };

        // Assert
        dto.DoctorId.Should().Be(1);
        dto.Name.Should().Be("Dr. House");
        dto.ChargesPerVisit.Should().Be(600.0);
        dto.ReputeIndex.Should().Be(4.8);
        dto.PatientsTreated.Should().Be(200);
        dto.Age.Should().Be(50);
        dto.Department.Should().Be("General");
    }

    [Fact]
    public void DoctorDto_DefaultConstructor_HasEmptyStrings()
    {
        // Act
        var dto = new DoctorDto();

        // Assert
        dto.Name.Should().Be(string.Empty);
    }

    // ─── DoctorListDto ────────────────────────────────────────────────────────

    [Fact]
    public void DoctorListDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new DoctorListDto
        {
            DoctorId = 3,
            Name = "Dr. Wilson",
            Department = "Oncology"
        };

        // Assert
        dto.DoctorId.Should().Be(3);
        dto.Name.Should().Be("Dr. Wilson");
        dto.Department.Should().Be("Oncology");
    }

    // ─── AddDoctorDto ─────────────────────────────────────────────────────────

    [Fact]
    public void AddDoctorDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new AddDoctorDto
        {
            Name = "Dr. New",
            Email = "new@clinic.com",
            Password = "pass123",
            BirthDate = "1985-05-10",
            DeptNo = 1,
            Phone = "9876543210",
            Gender = 'M',
            Address = "456 Oak Ave",
            WorkExperience = 10,
            Salary = 80000,
            ChargesPerVisit = 400,
            Specialization = "Neurology",
            Qualification = "MBBS, MD"
        };

        // Assert
        dto.Name.Should().Be("Dr. New");
        dto.Email.Should().Be("new@clinic.com");
        dto.DeptNo.Should().Be(1);
        dto.Gender.Should().Be('M');
        dto.WorkExperience.Should().Be(10);
        dto.Salary.Should().Be(80000);
        dto.ChargesPerVisit.Should().Be(400);
    }

    // ─── LoginResultDto ───────────────────────────────────────────────────────

    [Fact]
    public void LoginResultDto_SuccessResult_HasCorrectValues()
    {
        // Arrange & Act
        var dto = new LoginResultDto
        {
            Success = true,
            UserId = 5,
            UserType = 1,
            Message = "Login successful."
        };

        // Assert
        dto.Success.Should().BeTrue();
        dto.UserId.Should().Be(5);
        dto.UserType.Should().Be(1);
        dto.Message.Should().Be("Login successful.");
    }

    [Fact]
    public void LoginResultDto_FailureResult_HasCorrectValues()
    {
        // Arrange & Act
        var dto = new LoginResultDto
        {
            Success = false,
            Message = "Invalid email or password."
        };

        // Assert
        dto.Success.Should().BeFalse();
        dto.UserId.Should().Be(0);
        dto.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public void LoginResultDto_DefaultConstructor_HasEmptyMessage()
    {
        // Act
        var dto = new LoginResultDto();

        // Assert
        dto.Message.Should().Be(string.Empty);
        dto.Success.Should().BeFalse();
    }

    // ─── LoginRequestDto ──────────────────────────────────────────────────────

    [Fact]
    public void LoginRequestDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new LoginRequestDto
        {
            Email = "user@clinic.com",
            Password = "securepass"
        };

        // Assert
        dto.Email.Should().Be("user@clinic.com");
        dto.Password.Should().Be("securepass");
    }

    [Fact]
    public void LoginRequestDto_DefaultConstructor_HasEmptyStrings()
    {
        // Act
        var dto = new LoginRequestDto();

        // Assert
        dto.Email.Should().Be(string.Empty);
        dto.Password.Should().Be(string.Empty);
    }

    // ─── PatientSignupDto ─────────────────────────────────────────────────────

    [Fact]
    public void PatientSignupDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new PatientSignupDto
        {
            Name = "Jane Smith",
            BirthDate = "1995-07-20",
            Email = "jane@example.com",
            Password = "pass123",
            PhoneNo = "5551234567",
            Gender = "F",
            Address = "789 Pine St"
        };

        // Assert
        dto.Name.Should().Be("Jane Smith");
        dto.BirthDate.Should().Be("1995-07-20");
        dto.Email.Should().Be("jane@example.com");
        dto.Password.Should().Be("pass123");
        dto.PhoneNo.Should().Be("5551234567");
        dto.Gender.Should().Be("F");
        dto.Address.Should().Be("789 Pine St");
    }

    // ─── PatientDto ───────────────────────────────────────────────────────────

    [Fact]
    public void PatientDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new PatientDto
        {
            PatientId = 10,
            Name = "John Doe",
            Phone = "1234567890",
            Address = "123 Main St",
            BirthDate = "1990-01-15",
            Age = 34,
            Gender = "M"
        };

        // Assert
        dto.PatientId.Should().Be(10);
        dto.Name.Should().Be("John Doe");
        dto.Age.Should().Be(34);
        dto.Gender.Should().Be("M");
    }

    // ─── PatientListDto ───────────────────────────────────────────────────────

    [Fact]
    public void PatientListDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new PatientListDto
        {
            PatientId = 5,
            Name = "Alice Brown",
            Phone = "9998887777"
        };

        // Assert
        dto.PatientId.Should().Be(5);
        dto.Name.Should().Be("Alice Brown");
        dto.Phone.Should().Be("9998887777");
    }

    // ─── StaffDto ─────────────────────────────────────────────────────────────

    [Fact]
    public void StaffDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new StaffDto
        {
            StaffId = 3,
            Name = "Bob Johnson",
            Phone = "5554443333",
            Address = "321 Elm St",
            Gender = "M",
            Designation = "Lab Tech",
            Salary = 38000.0
        };

        // Assert
        dto.StaffId.Should().Be(3);
        dto.Name.Should().Be("Bob Johnson");
        dto.Designation.Should().Be("Lab Tech");
        dto.Salary.Should().Be(38000.0);
    }

    // ─── StaffListDto ─────────────────────────────────────────────────────────

    [Fact]
    public void StaffListDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new StaffListDto
        {
            StaffId = 8,
            Name = "Carol White",
            Designation = "Receptionist"
        };

        // Assert
        dto.StaffId.Should().Be(8);
        dto.Name.Should().Be("Carol White");
        dto.Designation.Should().Be("Receptionist");
    }

    // ─── AddStaffDto ──────────────────────────────────────────────────────────

    [Fact]
    public void AddStaffDto_SetProperties_ReturnsCorrectValues()
    {
        // Arrange & Act
        var dto = new AddStaffDto
        {
            Name = "New Staff",
            BirthDate = "1992-03-15",
            Phone = "1112223333",
            Gender = 'F',
            Address = "100 Maple Ave",
            Salary = 32000,
            Qualification = "BSc",
            Designation = "Pharmacist"
        };

        // Assert
        dto.Name.Should().Be("New Staff");
        dto.BirthDate.Should().Be("1992-03-15");
        dto.Gender.Should().Be('F');
        dto.Salary.Should().Be(32000);
        dto.Designation.Should().Be("Pharmacist");
    }

    [Fact]
    public void AddStaffDto_DefaultConstructor_HasEmptyStrings()
    {
        // Act
        var dto = new AddStaffDto();

        // Assert
        dto.Name.Should().Be(string.Empty);
        dto.Designation.Should().Be(string.Empty);
        dto.Qualification.Should().Be(string.Empty);
    }
}
